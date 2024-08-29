namespace MultiAgentsDatabaseCoPilot;
public record struct AppUser(
    DateTime Date_t,
    decimal AppStore_M_users,
    decimal Facebook_M_users,
    decimal Instagram_M_users,
    decimal Netflix_M_users,
    decimal PlayStation_M_users,
    decimal SnapChat_M_users,
    decimal TikTok_M_users,
    decimal Twitter_M_users,
    decimal WhatsApp_M_users,
    decimal YouTube_M_users
);

public record struct DataFxd(
    DateTime date_t,
    decimal perceived_call_success_rate,
    decimal perceived_call_drop_rate
);

public record struct GamingFxd(
    DateTime Date_t,
    decimal TRAFFIC_TB,
    int USERS
);

public record struct ResidentAnalytics(
    long MSISDN,
    string TOP_INTEREST,
    string HOME_CITY_NAME,
    string HOME_HAI_NAME,
    string OFFICE_CITY_NAME,
    string OFFICE_HAI_NAME,
    string WEALTH_INDEX,
    string PRIMARY_BANK,
    decimal TOTAL_MONTHLY_SPENDING,
    string ROAMING_COUNTRY,
    decimal INSTAGRAM_DOWNLOAD_VOL,
    decimal INSTAGRAM_UPLOAD_VOL,
    int INSTAGRAM_CLICK_COUNT,
    decimal TWITTER_DOWNLOAD_VOL,
    decimal TWITTER_UPLOAD_VOL,
    int TWITTER_CLICK_COUNT,
    decimal SNAPCHAT_DOWNLOAD_VOL,
    decimal SNAPCHAT_UPLOAD_VOL,
    int SNAPCHAT_CLICK_COUNT,
    decimal TIKTOK_DOWNLOAD_VOL,
    decimal TIKTOK_UPLOAD_VOL,
    int TIKTOK_CLICK_COUNT,
    DateTime OBS_DATE
);

public record struct RoamerData(
    DateTime TXN_DT,
    string NATIONAL_FLAG,
    string OPRTR_Cntry,
    int TXN_DUR,
    int TXN_CNT,
    decimal INC_DATA_VOL,
    decimal OUT_DATA_VOL,
    long MSISDN,
    string roaming_type,
    string usage_type
);

public record struct RoamingFxd(
    DateTime date_t,
    decimal Int_IB_Traffic_TB,
    int Int_IB_Roaming_Numbers,
    decimal Int_OB_Traffic_TB,
    int Int_OB_Roaming_Numbers
);

public record struct TrafficFxd(
    DateTime date_t,
    decimal _5G_Traffic_TB,
    int _5G_Total_Users_K,
    decimal _4G_Traffic_TB,
    int _4G_Total_Users_k
);

public record struct SalesFact( DateTime Date, 
                        int ProductID, 
                        string ProductCategory, 
                        int SalesQuantity,
                        decimal SalesAmount,
                        string CustomerGender,
                        string CustomerCountry,
                        int CustomerAge);
