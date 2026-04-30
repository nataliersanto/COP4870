using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Library.PoS.Services
{
    public class EmailService
    {
        //singleton pattern
        private static EmailService? instance;
        public static EmailService Current => instance ??= new EmailService();

        // SMTP settings instead of config
        private string smtpHost = "smtp.gmail.com";
        private int smtpPort = 587;
        private string senderEmail = "your-email@gmail.com";
        private string senderPassword = "your-app-password";
        private string senderName = "LMS System";

        //sends an email notif to students for new assignments
        public void NotifyStudentNewAssignment(string studentEmail, string studentName, string courseName, string assignmentName, DateTime dueDate)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(studentName, studentEmail));
                message.Subject = $"New Assignment Posted: {assignmentName}";

                message.Body = new TextPart("plain")
                {
                    Text = $"Hi {studentName},\n\n" +
                           $"A new assignment has been posted in {courseName}:\n\n" +
                           $"Assignment: {assignmentName}\n" +
                           $"Due Date: {dueDate:MM/dd/yyyy}\n\n" +
                           $"Please log in to the LMS to view and submit your work.\n\n" +
                           $"Best,\nLMS System"
                };

                using var client = new SmtpClient();
                client.Connect(smtpHost, smtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(senderEmail, senderPassword);
                client.Send(message);
                client.Disconnect(true);

                Console.WriteLine($"Email sent to {studentEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email notification failed: {ex.Message}"); //just log error
            }
        }
        public void NotifyCourseStudents(List<Model.Student> roster, string courseName, string assignmentName, DateTime dueDate)
        {
            foreach (var student in roster)
            {
                var studentEmail = $"{student.Code}@fsu.edu";
                NotifyStudentNewAssignment(studentEmail, student.Name ?? "Student", courseName, assignmentName, dueDate);
            }
        }
    }
}