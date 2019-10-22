using System;

namespace Logger.Model
{
    public class LogModel
    {
        public DateTime Date { get => DateTime.Now.Date; }
        public string Time { get => DateTime.Now.ToLongTimeString(); }
        public string ClassName { get; set; }
        public Validator.ValidationResult Result { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
