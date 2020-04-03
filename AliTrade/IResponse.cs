using System;
using System.Collections.Generic;

namespace AliTrade
{
    public abstract class TaeResponse
    {
        public string error_code { get; set; }
        public string error_msg { get; set; }
        public string Body { get; set; }
        public ErrorResponse TopErrResp { get; set; }

        public bool IsError { get { return !string.IsNullOrEmpty(this.error_code); } }
    }
    public class ErrorResponse
    {
        public int code { get; set; }
        public string msg { get; set; }
        public string sub_code { get; set; }
        public string sub_msg { get; set; }
        public string request_id { get; set; }
    }

    #region taobao.tbk.order.details.get
    public class TbkOrderDetailsQueryResponse : TaeResponse
    {
        public TbkOrderDetailsQueryData data { get; set; }
    }
    public class TbkOrderDetailsQueryData
    {
        public bool has_next { get; set; }
        public bool has_pre { get; set; }
        public int page_no { get; set; }
        public int page_size { get; set; }
        public string position_index { get; set; }
        public List<TbkOrderDetailsQueryResult> results { get; set; }
    }
    public class TbkOrderDetailsQueryResult
    {
        /// <summary>
        /// 买家通过购物车购买的每个商品对应的订单编号，此订单编号并未在淘宝买家后台透出
        /// </summary>
        public long trade_id { get; set; }
        /// <summary>
        /// 买家在淘宝后台显示的订单编号
        /// </summary>
        public long trade_parent_id { get; set; }

        /// <summary>
        /// 通过推广链接达到商品、店铺详情页的点击时间
        /// </summary>
        public DateTime click_time { get; set; }
        /// <summary>
        /// 订单创建的时间，该时间同步淘宝，可能会略晚于买家在淘宝的订单创建时间
        /// </summary>
        public DateTime tk_create_time { get; set; }
        /// <summary>
        /// 单确认收货后且商家完成佣金支付的时间
        /// </summary>
        public DateTime? tk_earning_time { get; set; }
        /// <summary>
        /// 订单所属平台类型，包括天猫、淘宝、聚划算等
        /// </summary>
        public string order_type { get; set; }
        /// <summary>
        /// 产品类型
        /// </summary>
        public string flow_source { get; set; }
        /// <summary>
        /// 已付款：指订单已付款，但还未确认收货 
        /// 已收货：指订单已确认收货，但商家佣金未支付 
        /// 已结算：指订单已确认收货，且商家佣金已支付成功 
        /// 已失效：指订单关闭/订单佣金小于0.01元，订单关闭主要有：1）买家超时未付款； 2）买家付款前，买家/卖家取消了订单；3）订单付款后发起售中退款成功；
        /// 3：订单结算，12：订单付款， 13：订单失效，14：订单成功
        /// </summary>
        public int tk_status { get; set; }


        /// <summary>
        /// 商品id
        /// </summary>
        public long item_id { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        public string item_title { get; set; }
        /// <summary>
        /// 商品图片
        /// </summary>
        public string item_img { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int item_num { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public decimal item_price { get; set; }
        /// <summary>
        /// 成交平台
        /// </summary>
        public string terminal_type { get; set; }


        /// <summary>
        /// 买家拍下付款的金额（不包含运费金额）
        /// </summary>
        public decimal alipay_total_price { get; set; }
        /// <summary>
        /// 买家确认收货的付款金额（不包含运费金额）
        /// </summary>
        public decimal pay_price { get; set; }

        /// <summary>
        /// 付款预估收入=付款金额*提成。指买家付款金额为基数，预估您可能获得的收入。因买家退款等原因，可能与结算预估收入不一致
        /// </summary>
        public decimal pub_share_pre_fee { get; set; }
        /// <summary>
        /// 订单结算的佣金比率+平台的补贴比率
        /// </summary>
        public decimal income_rate { get; set; }
        /// <summary>
        /// 从结算佣金中分得的收益比率
        /// </summary>
        public decimal pub_share_rate { get; set; }
        /// <summary>
        /// 结算预估收入=结算金额*提成。以买家确认收货的付款金额为基数，预估您可能获得的收入。因买家退款、您违规推广等原因，可能与您最终收入不一致。最终收入以月结后您实际收到的为准
        /// </summary>
        public decimal pub_share_fee { get; set; }
        /// <summary>
        /// 佣金比率
        /// </summary>
        public decimal total_commission_rate { get; set; }
        /// <summary>
        /// 佣金金额=结算金额*佣金比率
        /// </summary>
        public decimal total_commission_fee { get; set; }

        /// <summary>
        /// 会员运营id
        /// </summary>
        public long special_id { get; set; }
        /// <summary>
        /// 渠道关系id
        /// </summary>
        public long relation_id { get; set; }
        /// <summary>
        /// 推广位管理下的推广位名称对应的ID，同时也是pid=mm_1_2_3中的“3”这段数字
        /// </summary>
        public long adzone_id { get; set; }
    }
    #endregion

}