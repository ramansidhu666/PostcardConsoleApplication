using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Data.Entity;
using System.Web.Configuration;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace PostcardConsoleApplication
{
   public class Program
    {
        static string Email = "";
        static void Main(string[] args)
        {

            //SqlConnection con = new SqlConnection("Data Source=74.208.69.145;Initial Catalog=CommunicationAppCopy;User ID=sa;Password=!nd!@123");
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["comm_conn"].ConnectionString.ToString());

            SqlDataAdapter adp = new SqlDataAdapter("select * from customer where IsActive=1", con);
            DataTable dt = new DataTable();
            adp.Fill(dt);

            DateTime Datetim = DateTime.Now;
            if (Datetim.Month == 12 && Datetim.Day == 25)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var Email = row["EmailId"].ToString();
                    MerryCrismasDay(Email);
                }
            }
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    if (row["DOB"] != System.DBNull.Value && row["DOB"] != "")
                    {

                         Email = row["EmailId"].ToString();
                        var Photopath = row["PhotoPath"].ToString();
                        var Name = row["FirstName"].ToString() + " " + row["LastName"].ToString();


                        //user date of birth
                        var dateofbirth = row["DOB"].ToString();
                        var splitdate = dateofbirth.Split('/');
                        if (splitdate.Length==3)
                        {
                            if (Convert.ToInt32(splitdate[0]) > 12)
                            {
                                dateofbirth = splitdate[1] + "/" + splitdate[0] + "/" + splitdate[2];
                            }

                            DateTime Dt = Convert.ToDateTime(dateofbirth);
                            string output = Dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            DateTime Dob = DateTime.ParseExact(output, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            var dob_day = Dob.Month;
                            var dob_month = Dob.Day;
                            //End

                            //Today date
                            var currentdate = DateTime.Now;
                            var current_day = currentdate.Day;
                            var current_month = currentdate.Month;
                            //End

                            if (current_month == dob_month)
                            {
                                if (current_day == dob_day)
                                {

                                    Sendmail(Email, Name, Photopath, dateofbirth.ToString());
                                    Console.WriteLine("Send mail to " + Email + " Id " + row["CustomerId"].ToString());
                                }

                            }
                        }
                        

                       


                    }
                }
                catch(Exception ex)
                {
                    SqlConnection rajpal_conn = new SqlConnection(ConfigurationManager.ConnectionStrings["rajpal_conn"].ConnectionString.ToString());


                    SqlDataAdapter adapter = new SqlDataAdapter();
                    string sql = null;

                    sql = "insert into errorlog ([DBname],[message],[EmailId]) values('BirthdayApp','" + ex.Message.ToString() + "in main function. " + "','" + Email + "')";
                    try
                    {
                        rajpal_conn.Open();
                        adapter.InsertCommand = new SqlCommand(sql, rajpal_conn);
                        adapter.InsertCommand.ExecuteNonQuery();

                    }
                    catch (Exception ex1)
                    {
                        rajpal_conn.Close();
                    }
                }
                

            }


        }


        public static void Sendmail(string Email,string Name,string Photopath,string Dob)
         {
            try
            {
                //Send mail
                MailMessage mail = new MailMessage();
                //Email = "only4agentss@gmail.com";
                string FromEmailID = ConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"];

                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(Email));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "B'day greetings.";
                string msgbody = "";
                var Template = "";
                Template = "Templates/FirstPostCard.html";


                using (StreamReader reader = new StreamReader(@"C:\sites\PostcardConsoleApplication\PostcardConsoleApplication\Templates\FirstPostCard.html"))
                //using (StreamReader reader = new StreamReader(Path.Combine(HttpRuntime.AppDomainAppPath, Template)))
                {
                    msgbody = reader.ReadToEnd();
                    msgbody = msgbody.Replace("{PhotoPath}", Photopath);
                    msgbody = msgbody.Replace("{Name}", Name);
                    msgbody = msgbody.Replace("{DOB}", Dob);

                }

                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(msgbody, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(msgbody, null, "text/html");

                mail.AlternateViews.Add(plainView);
                mail.AlternateViews.Add(htmlView);
                // mail.Body = msgbody;
                mail.IsBodyHtml = true;



                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                SqlConnection rajpal_conn = new SqlConnection(ConfigurationManager.ConnectionStrings["rajpal_conn"].ConnectionString.ToString());


                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;

                sql = "insert into errorlog ([DBname],[message],[EmailId]) values('BirthdayApp','" + ex.Message.ToString() + "in send mail function." + "','" + Email + "')";
                try
                {
                    rajpal_conn.Open();
                    adapter.InsertCommand = new SqlCommand(sql, rajpal_conn);
                    adapter.InsertCommand.ExecuteNonQuery();

                }
                catch (Exception ex1)
                {
                    rajpal_conn.Close();
                }
            }
            
        }


        public static void MerryCrismasDay(string Email)
        {
            try
            {
                //Send mail
                MailMessage mail = new MailMessage();
                // Email = "only4agentss@gmail.com";
                string FromEmailID = ConfigurationManager.AppSettings["FromEmailID"];
                string FromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"];

                SmtpClient smtpClient = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                int _Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString());
                Boolean _UseDefaultCredentials = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDefaultCredentials"].ToString());
                Boolean _EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"].ToString());
                mail.To.Add(new MailAddress(Email));
                mail.From = new MailAddress(FromEmailID);
                mail.Subject = "Merry Christmas ";
                string msgbody = "";

                using (StreamReader reader = new StreamReader(@"C:\sites\PostcardConsoleApplication\PostcardConsoleApplication\Templates\MerryChrismas.html"))
                {
                    msgbody = reader.ReadToEnd();
                }

                mail.BodyEncoding = System.Text.Encoding.UTF8;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                System.Net.Mail.AlternateView plainView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(System.Text.RegularExpressions.Regex.Replace(msgbody, @"<(.|\n)*?>", string.Empty), null, "text/plain");
                System.Net.Mail.AlternateView htmlView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(msgbody, null, "text/html");

                mail.AlternateViews.Add(plainView);
                mail.AlternateViews.Add(htmlView);
                // mail.Body = msgbody;
                mail.IsBodyHtml = true;



                SmtpClient smtp = new SmtpClient();
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = "smtp.gmail.com"; //_Host;
                smtp.Port = _Port;
                //smtp.UseDefaultCredentials = _UseDefaultCredentials;
                smtp.Credentials = new System.Net.NetworkCredential(FromEmailID, FromEmailPassword);// Enter senders User name and password
                smtp.EnableSsl = _EnableSsl;
                smtp.Send(mail);
            }
            catch(Exception ex)
            {
                SqlConnection rajpal_conn = new SqlConnection(ConfigurationManager.ConnectionStrings["rajpal_conn"].ConnectionString.ToString());


                SqlDataAdapter adapter = new SqlDataAdapter();
                string sql = null;

                sql = "insert into errorlog ([DBname],[message],[EmailId]) values('BirthdayApp','" + ex.Message.ToString() + "in christmas function. " + "','" + Email + "')";
                try
                {
                    rajpal_conn.Open();
                    adapter.InsertCommand = new SqlCommand(sql, rajpal_conn);
                    adapter.InsertCommand.ExecuteNonQuery();

                }
                catch (Exception ex1)
                {
                    rajpal_conn.Close();
                }
            }
        }
    }

   
}
