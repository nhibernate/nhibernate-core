namespace NHibernate.Validator
{
    using System;

    [Serializable]
    public class InvalidValue
    {
        private readonly string message;
        private readonly Object value;
        private readonly Type beanClass;
        private readonly String propertyName;
        private readonly Object bean;
        private Object rootBean;
        private String propertyPath;

        public InvalidValue(String message, Type beanClass, String propertyName, Object value, Object bean)
        {
            this.message = message;
            this.value = value;
            this.beanClass = beanClass;
            this.propertyName = propertyName;
            this.bean = bean;
            rootBean = bean;
            propertyPath = propertyName;
        }

        public void AddParentBean(Object parentBean, String propertyName) 
        {
            rootBean = parentBean;
            propertyPath = propertyName + "." + propertyPath;
        }

        public Object RootBean
        {
            get { return rootBean; }
        }

        public String PropertyPath
        {
            get { return propertyPath; }
        }

        public Type BeanClass
        {
            get { return beanClass; }
        }

        public String Message
        {
            get { return message; }
        }

        public String PropertyName
        {
            get { return propertyName; }
        }

        public Object Value
        {
            get { return value; }
        }

        public Object Bean
        {
            get { return bean; }
        }

        public override string ToString()
        {
            return string.Concat(propertyName, " ", message);
        }
    }
}