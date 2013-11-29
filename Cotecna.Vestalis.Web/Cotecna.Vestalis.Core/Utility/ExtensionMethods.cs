
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Reflection;
namespace Cotecna.Vestalis.Core
{
    public static class ExtensionMethods
    {

        /// <summary>
        /// Get the value of the property name
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="entity">Entity of the property</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>The value of the property</returns>
        public static T GetPropertyValue<T>(this object entity, string propertyName)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            var type = entity.GetType();
            var propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The class {0} doesn't have a property named {1}.", type.ToString(), propertyName));
            }
            return (T)propertyInfo.GetValue(entity, null);
        }

        /// <summary>
        /// Convert a FormCollection to a dictionary only with the keys that have value
        /// </summary>
        /// <param name="collection">FormCollection</param>
        /// <returns>Dictionary</returns>
        public static Dictionary<string, string> ToFilledDictionary(this FormCollection collection)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string key in collection.AllKeys)
            {
                if (!string.IsNullOrEmpty(collection[key]))
                    result.Add(key, collection[key]);
            }
            return result;
        }

        /// <summary>
        /// Convert a MembershipUserCollection in generic list of MembershipUser
        /// </summary>
        /// <param name="collection">Collection to convert</param>
        /// <returns>List of MembershipUser</returns>
        public static List<MembershipUser> ToList(this MembershipUserCollection collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");
            List<MembershipUser> result = new List<MembershipUser>();
            foreach (MembershipUser item in collection)
            {
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Get property according to string property using reflexion
        /// </summary>
        /// <typeparam name="T">Type to serach property</typeparam>
        /// <param name="field">string field name</param>
        /// <returns>entity property</returns>
        public static Func<T, object> GetField<T>(string field)
        {
            PropertyInfo propertyInfo = typeof(T).GetProperty(field);
            //get the type of teh property
            Type propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType)
                 ?? propertyInfo.PropertyType;

            //Return a function that handle the cast for ordering
            return obj =>
            {
                //Get the value of the property
                var value = propertyInfo.GetValue(obj, null);
                //Return the value casting when it is not null.
                return value == null ? null : (Convert.ChangeType(value, propertyType));
            };
        }
    }
}

