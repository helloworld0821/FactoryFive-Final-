using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.Model
{    
    /// <summary>
     /// 储层参数模板Model
     /// </summary>
    class CCCSMBModel
    {
        /// <summary>
        /// 油层组名称
        /// 砂体类型
        /// 孔隙度
        /// 饱和度
        /// 渗透率
        /// </summary>
        public string yczmc { set; get; }
        public string stlx { set; get; }
        public double kxd { set; get; }
        public double bhd { set; get; }
        public double stl { set; get; }
    }
}
