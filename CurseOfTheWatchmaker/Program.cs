/*Copyright Hermein Developer
 License CC BY-SA 4.0 
 23.04.2018 */
using Newtonsoft.Json.Linq;
using System;

namespace CurseOfTheWatchmaker
{
    class Program
    {
        public static Random rand = new Random();
        static void Main(string[] args)
        {
            int i = 0;
            string uid, auth;

            Console.WriteLine("Enter your viewer_id");
            uid = Console.ReadLine();
            if (!int.TryParse(uid, out i))
            {
                Console.WriteLine("Wrong viewer_id");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Enter your auth_key");
            auth = Console.ReadLine();
            if (auth.Length != 32)
            {
                Console.WriteLine("Wrong auth_key");
                Console.ReadKey();
                return;
            }
            i = 0;
            Console.WriteLine("Enter Ruby Count (10-10000)");
            var rubyCnt = Console.ReadLine();
            if (!int.TryParse(rubyCnt, out i))
            {
                Console.WriteLine("Wrong Ruby Count");
                Console.ReadKey();
                return;
            }
            if (i < 10 || i > 10000)
            {
                Console.WriteLine("Wrong Ruby Count (10-10000)");
                Console.ReadKey();
                return;
            }

            //auth
            var vkApi = Api.Get("https://cm-prod-vk.belkatechnologies.com/bejeweled-balancer/abtest_container_proxy_prod?api_url=https://api.vk.com/api.php&api_id=3088991&api_settings=3&viewer_id=" + uid + "&viewer_type=2&sid=&secret=&access_token=&user_id=" + uid + "&group_id=0&is_app_user=1&auth_key=" + auth + "&language=0&parent_language=0&is_secure=1&stats_hash=&ads_app_id=&referrer=unknown&lc_name=&hash=");
            vkApi = vkApi.Replace(" ", "");
            var abTest = Api.Parse("initialflashParams.abTest=\"([0-9]+)\";", vkApi);
            if (string.IsNullOrWhiteSpace(abTest))
            {
                Console.WriteLine("Wrong id/auth_key (not auth)");
                Console.ReadKey();
                return;
            }
            //init
            var timeS = Api.Parse("initialflashParams.timeS='([0-9a-zA-Z]+)';", vkApi);
            var _hash = timeS.Substring(timeS.Length - 5, 4);
            var _timeStart = long.Parse(timeS.Substring(0, timeS.Length - 5));
            var r = rand.Next(0, 9).ToString();
            var state = Api.Post("https://wcm-vk.belka-games.com/BUS/adapters/state", "table=state&id=" + uid + "&method=state&game=wcm&sn=vk&s=" + r + Api.Md5(_hash + r + _timeStart / 61 + "state").Substring(3, 4));
            try
            {
                var jt = JObject.Parse(state);
                var sv = jt["serverVersion"].Value<string>();
                var sess = jt["sessionId"].Value<long>();
                var req = jt["requestID"].Value<long>();
                var ruby = jt["state"]["consumables"]["ruby"].Value<long>();
                var dopRuby = i + ruby;
                //crack
                var post = "set!consumables.ruby!" + dopRuby + "" + sess;//set!res.17321
                r = rand.Next(0, 9).ToString();
                var shash = r + Api.Md5(_hash + r + (_timeStart / 61) + "" + req + "state" + post).Substring(3, 4);
                var pack = Api.Post("https://wcm-vk.belka-games.com/BUS/adapters/pack", "cmd=set!consumables.ruby!" + dopRuby + "&sessionId=" + sess + "&table=state&id=" + uid + "&serverVersion=" + sv + "&method=state&game=wcm&sn=vk&reqId=" + req + "&s=" + shash);
                if (pack == req.ToString())
                {
                    Console.WriteLine("Succsess!!! Please reboot game!");
                    Console.ReadKey();
                    return;
                }
                else
                {
                    Console.WriteLine(pack);
                    Console.ReadKey();
                    return;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                return;
            }
        }
        
    }
}
