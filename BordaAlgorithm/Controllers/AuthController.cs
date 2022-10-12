using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BordaAlgorithm.Models;
using BordaAlgorithm.Utilities;
using BordaAlgorithm.ViewModels;
using Microsoft.AspNet.Identity;

namespace BordaAlgorithm.Controllers
{
    public class AuthController : Controller
    {
        DBBordaAlgorithmEntities db = new DBBordaAlgorithmEntities();

        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(DataUserViewModel model)
        {
            ModelState.Remove("result");
            if (!ModelState.IsValid) return View(model);

            var userData = db.Users.Where(r => r.Username.ToUpper() == model.Username.ToUpper() && r.Is_Deleted != true).FirstOrDefault();
            if (userData != null)
            {
                ModelState.AddModelError(Constants._ERROR, "User has been existed!");
                return View(model);
            }
            else
            {
                if(model.Password!=model.ConfirmPassword)
                {
                    ModelState.AddModelError(Constants._ERROR, "Password and confirmation doesn't match!");
                    return View(model);
                }

                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        User newUser = new User();
                        newUser.Username = model.Username;
                        newUser.Longname = model.Longname;
                        newUser.Is_Admin = false;
                        newUser.Career = model.Career;
                        newUser.Password = UEncryption.Encrypt(model.Password);
                        db.Entry(newUser).State = EntityState.Added;
                        db.SaveChanges();
                        trans.Commit();
                        model.result = "OK";
                    }
                    catch(Exception ex)
                    {
                        ModelState.AddModelError(Constants._ERROR, "Failed to save new user!");
                        trans.Rollback();
                    }
                }
                
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string ReturnUrl)
        {
            ViewBag.ReturnUrl = string.IsNullOrEmpty(returnUrl) ? ReturnUrl : returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid) return View(model);

            if (!string.IsNullOrEmpty(model.Username))
                model.Username = model.Username.ToUpper();

            var userData = db.Users.Where(r => r.Username == model.Username && r.Is_Deleted != true).FirstOrDefault();
            if(userData!=null)
            {
                string password = UEncryption.Decrypt(userData.Password);
                if (UEncryption.Decrypt(userData.Password) == model.Password)
                {
                    bool isAdmin = userData.Is_Admin == null || userData.Is_Admin==false? false : true;
                    SignInAsync(userData.Username, userData.Longname, isAdmin);
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    try
                    {
                        userData.Last_Login = DateTime.Now;
                        db.Entry(userData).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        ModelState.AddModelError(Constants._ERROR, "Failed to update last login user!");
                        return View(model);
                    }
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(Constants._ERROR, "Invalid username or password!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError(Constants._ERROR, "Invalid username or password!");
                return View(model);
            }
        }
        private void SignInAsync(string userName, string longName, bool isAdmin)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
            claims.Add(new Claim(Constants.USERNAME, userName));
            claims.Add(new Claim(Constants.NAME, longName));
            claims.Add(new Claim(Constants.IS_ADMIN, isAdmin ? "1" : "0"));
            var id = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignIn(id);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            authenticationManager.SignOut();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private static string Decrypt(string strEncrypted)
        {
            try
            {
                strEncrypted = strEncrypted.Replace("wilmarKey", "&");
                return Decrypt(strEncrypted, "wilmarKey");
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        private static string Decrypt(string strEncrypted, string strKey)
        {
            try
            {
                TripleDESCryptoServiceProvider objDESCrypto =
                    new TripleDESCryptoServiceProvider();
                MD5CryptoServiceProvider objHashMD5 = new MD5CryptoServiceProvider();
                byte[] byteHash, byteBuff;
                string strTempKey = strKey;
                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB
                byteBuff = Convert.FromBase64String(strEncrypted);
                string strDecrypted = ASCIIEncoding.ASCII.GetString
                (objDESCrypto.CreateDecryptor().TransformFinalBlock
                (byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;
                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            var username = User.Identity.GetUserDataByKey(Constants.USERNAME);
            var message = "";
            var result = false;
            var oldPassword = model.old_password;
            var newPassword = model.new_password;
            var confirmPassword = model.confirm_password;

            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    throw new Exception("Unable to change password");
                }

                var userData = db.Users.Where(r => r.Username == username && (bool)r.Is_Deleted!=true).FirstOrDefault();
                if (userData != null)
                {
                    string testPassword = UEncryption.Decrypt(userData.Password);

                    if (testPassword != oldPassword)
                    {
                        throw new Exception("Wrong Old Password");
                    }

                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            userData.Password = UEncryption.Encrypt(newPassword);
                            db.Entry(userData).State = EntityState.Modified;
                            db.SaveChanges();
                            trans.Commit();
                            message = "Password successfully changed.";
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new Exception("Unable to change password");
                        }
                    }
                }
                else
                {
                    throw new Exception("Unable to change password");
                }
            }
            catch (Exception ex)
            {
                result = false;
                message = ex.Message;
            }

            return Json(new
            {
                success = result,
                message = message
            });
        }
    }
}
public class ChangePasswordModel
{
    public string old_password { get; set; }
    public string new_password { get; set; }
    public string confirm_password { get; set; }
}