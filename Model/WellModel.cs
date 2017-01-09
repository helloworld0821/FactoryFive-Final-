using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXApplication1.Model
{
    class WellModel
    {
        /// <summary>
        /// 油井坐标信息
        /// mncy - 模拟产油
        /// sumcy - 模拟分层产油求和
        /// mncs - 模拟产水
        /// sumcs - 模拟分层产水求和
        /// cye - 产液
        /// sumcye - 模拟分层产液求和
        /// 
        /// </summary>
        public string sjjhy { set; get; }
        public string jhy { set; get; }
        public string cw { set; get; }
        public double HS { set; get; }
        public double mncye { set; get; }
        public double CYEQD { set; get; }
        public double YSYHD { set; get; }
        public double YSTL { set; get; }
        public double YWX { set; get; }
        public string sjjhs { set; get; }
        public string jhs { set; get; }
        public double ZRQD { set; get; }
        public double SSYHD { set; get; }
        public double SSTL { set; get; }
        public double SWX { set; get; }
        public string szj { set; get; }
        public double szjnd { set; get; }
        public string date { set; get; }
        public DateTime dtDate { set; get; }
        public int x { set; get; }
        public int y { set; get; }
        public int k { set; get; }
        public double mncy { set; get; }
        public double mncs { set; get; }
        public double ZRQDHI { set; get; }
        public double YYX { set; get; }
        public double YKH { set; get; }
        public double SKH { set; get; }
        public double SYX { set; get; }
        public double ZRBFS { set; get; }
        public double ZRBFSHI { set; get; }
        public double CYEQDHI { set; get; }
        public double HSHI { set; get; }
        public double RCYLHI { set; get; }
        public double RCYLBFS { set; get; }
    }
}
