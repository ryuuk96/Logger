namespace MultiLogger.Writers.Models
{
    public class MultiLogHelper
    {
        public enum ProjectType
        {
            Shopify,
            BigCommerce,
            WebSite,
            Common,
            Microsoft
        }
        public enum UserType
        {
            Application,
            Developer,
            Admin,
            Customer
        }
        public enum OperationType
        {
            CREATE,
            READ,
            UPDATE,
            DELETE,
            ERROR,
            EXCEP,
            CONN,
            DISCONN,
        }
    }
}
