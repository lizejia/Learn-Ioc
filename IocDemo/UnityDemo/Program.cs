using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //构造器注入、属性（设置）注入和接口注入

            //Unity API

            /*UnityContainer.RegisterType<ITFrom,TTO>();

            UnityContainer.RegisterType<ITFrom, TTO>();

            UnityContainer.RegisterType<ITFrom, TTO>("keyName");

            IEnumerable<T> databases = UnityContainer.ResolveAll<T>();

            IT instance = UnityContainer.Resolve<IT>();

            T instance = UnityContainer.Resolve<T>("keyName");

            UnitContainer.RegisterInstance<T>("keyName", new T());

            UnityContainer.BuildUp(existingInstance);

            IUnityContainer childContainer1 = parentContainer.CreateChildContainer();*/


            //1用编程方式实现注入
            Console.WriteLine("***********1用编程方式实现注入*************");
            ContainerCode();
            //2配置文件注入 
            Console.WriteLine("***********2配置文件注入App.config*************");
            ContainerConfiguration();
            //2.1配置文件注入 
            Console.WriteLine("***********2.1配置文件注入Unity.config*************");
            ContainerUnityConfiguration();
            Console.ReadKey();
        }


        /// <summary>
        /// 1、用编程方式实现注入
        /// 代码注入
        /// </summary>
        public static void ContainerCode()
        {

            //A、创建一个UnityContainer对象
            IUnityContainer container = new UnityContainer();

            //B、通过UnityContainer对象的RegisterType方法来注册对象与对象之间的关系
            container.RegisterType<IProduct, Milk>();  //默认注册（无命名）,如果后面还有默认注册会覆盖前面的
            container.RegisterType<IProduct, Sugar>("Sugar");  //命名注册

            //C、通过UnityContainer对象的Resolve方法来获取指定对象关联的对象
            IProduct _product = container.Resolve<IProduct>();  //解析默认对象
            _product.ClassName = _product.GetType().ToString();
            _product.ShowInfo();

            IProduct _sugar = container.Resolve<IProduct>("Sugar");  //指定命名解析对象
            _sugar.ClassName = _sugar.GetType().ToString();
            _sugar.ShowInfo();

            IEnumerable<IProduct> classList = container.ResolveAll<IProduct>(); //获取容器中所有IProduct的注册的已命名对象

            foreach (var item in classList)
            {
                item.ClassName = item.GetType().ToString();
                item.ShowInfo();
            }
        }

        /// <summary>
        /// 2、配置文件注入 
        /// C、在代码中读取配置信息，并将配置载入到UnityContainer中
        /// </summary>
        public static void ContainerConfiguration()
        {
            IUnityContainer container = new UnityContainer();
            //获取指定名称的配置节
            UnityConfigurationSection section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            //获取特定配置节下已命名的配置节<container name='MyContainer'>下的配置信息
            section.Configure(container, "MyContainer");
            container.LoadConfiguration("MyContainer");

            IProduct classInfo = container.Resolve<IProduct>("Sugar");
            classInfo.ClassName = "我屮艸芔茻";
            classInfo.ShowInfo();
            IProduct classInfo1 = container.Resolve<IProduct>();
            classInfo1.ClassName = "classInfo1";
            classInfo1.ShowInfo();
            
        }

        /// <summary> 
        /// 如果系统比较庞大，那么对象之间的依赖关系可能就会很复杂，最终导致配置文件变得很大，
        /// 所以我们需要将Unity的配置信息从App.config或web.config中分离出来到某一个单独的配置文件中，
        /// 比如Unity.config，实现方式可以参考如下代码
        /// </summary>
        public static void ContainerUnityConfiguration()
        {
            IUnityContainer container = new UnityContainer();

            string configFile = "Unity.config";
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            //从config文件中读取配置信息
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            //获取指定名称的配置节
            UnityConfigurationSection section = (UnityConfigurationSection)configuration.GetSection("unity");
            //获取特定配置节下已命名的配置节<container name='MyContainer'>下的配置信息
            section.Configure(container, "MyContainer");
            //载入名称为FirstClass 的container节点
            container.LoadConfiguration(section, "MyContainer");

            IProduct classInfo = container.Resolve<IProduct>("Sugar");
            classInfo.ClassName = "我屮艸芔茻";
            classInfo.ShowInfo();
            IProduct classInfo1 = container.Resolve<IProduct>();
            classInfo1.ClassName = "classInfo1";
            classInfo1.ShowInfo();

        }
    }

    /// <summary>
    /// 商品
    /// </summary>
    public interface IProduct
    {
        string ClassName { get; set; }
        void ShowInfo();
    }

    /// <summary>
    /// 牛奶
    /// </summary>
    public class Milk : IProduct
    {
        public string ClassName { get; set; }

        public void ShowInfo()
        {
            Console.WriteLine("牛奶：{0}", ClassName);
        }
    }
    /// <summary>
    /// 糖
    /// </summary>
    public class Sugar : IProduct
    {
        public string ClassName { get; set; }

        public void ShowInfo()
        {
            Console.WriteLine("糖：{0}", ClassName);
        }
    }
}
