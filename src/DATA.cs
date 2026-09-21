namespace woop_app
{

    public enum WhoopCommands
    {
        Buzz = 67,
        Get_Clock = 11,
        Get_Battery = 26,
        GET_BODY_LOCATION_AND_STATUS = 84,
        GET_ADVERTISING_NAME = 76,
        Set_Config = 0x78
    }

    public static class WhoopData{

        public static readonly Dictionary<string, byte> Configs = new Dictionary<string, byte>
        {
            { "enable_r22_packets", 0x32 },
            { "hr_ch_switching", 0x32 },
            { "enable_r22_v2_packets", 0x32 },
            { "ir_hw_switching", 0x32 },
            { "enable_r22_v3_packets", 0x32 },
            { "enable_passive_strap_fit_gen5", 0x31 },
            { "enable_r22_v4_packets", 0x31 },
            { "enable_sig11_during_sleep", 0x32 },
            { "enable_r22_v5_packets", 0x32 },
            { "dorset_inhibit_wpt", 0x32 },
            { "enable_r22_v6_packets", 0x32 },
            { "make_hrfm_visible", 0x32 },
            { "enable_r22_v8_packets", 0x32 },
            { "disable_pip_r26_packets", 0x32 },
            { "wear_detect_bias", 0x32 },
        };
    }
 
}
