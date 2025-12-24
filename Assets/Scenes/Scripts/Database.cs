using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Database : MonoBehaviour
{
    // Start is called before the first frame update


    public class parent
    {
        public string parent_id { get; set; }
        public child[] children { get; set; }

    }

    public class child
    {
        public string child_id { get; set; }
        public string name { get; set; }
        public task[] tasks { get; set; }

        public int main_wallet { get; set; }
        public int emergency_wallet { get; set; }
        public int emergency_wallet_target { get; set; }
        public item[] inventory { get; set; }
        public transaction_history[] history { get; set; }

    }
    public class task
    {
        public int type { get; set; }
        public string task_description { get; set; }
        public bool is_finished { get; set; }

    }

    public class transaction_history
    {
        public DateTime date { get; set; }
        public string transaction_description { get; set; }
        public int transaction_amount { get; set; }
    }

    public class item
    {
        public string item_id { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public int type { get; set; }
        public int category { get; set; }
        public int quantity { get; set; }

    }




}


