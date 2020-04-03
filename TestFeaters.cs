using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class TestFeaters
    {
        public void run() {
            
            var obj = new SomeClass(
                tag: "one",
                child: new SomeClass(
                    "sub1", 
                    child: new SomeClass("sub2")
                )
            );
            var objDeepChild = obj.child?.child ?? new SomeClass("two");

            var list = new List<int>() {0,1,2,3,4,5};
            list.ToArray().Print();

            var sublist0_1 = list.Take(2);
            sublist0_1.ToArray().Print();

            var sublistLast2 = list.TakeLast(2);
            sublistLast2.ToArray().Print();

            RunWithoutException(() => {
                var sublistOwerflow = list.Take(10);
                sublistOwerflow.ToArray().Print();
            });


            var allItems = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            allItems.Print();

            var firstItem = allItems[0];
            firstItem.Print();

            var secondItem = allItems[1];
            secondItem.Print();

            var lastItem1 = allItems[allItems.Length-1];
            lastItem1.Print();

            var lastItem2 = allItems[^1];
            lastItem2.Print();

            var items0to4 = allItems[0..4];
            items0to4.Print();

            var itemsTo4 = allItems[..4];
            itemsTo4.Print();

            var itemsFrom4ToEnd = allItems[4..];
            itemsFrom4ToEnd.Print();

            var copyAllItems = allItems[..];
            copyAllItems.Print();

            var index = ^0;
            index.Print();

            var range = 2..4;
            range.Print();

            RunWithoutException(() => {
                var outOfRangeItem = allItems[^0];
                outOfRangeItem.Print();
            });

            RunWithoutException(() => {
                var outOfRangeItems = allItems[6..14];
                outOfRangeItems.Print();
            });

        }

        private void RunWithoutException(Action action) {
            try {
                action.Invoke();
            } catch(Exception e) {
                Console.WriteLine(e);
            }
        }
    }

    static class Extensions {
        public static void Print<T>(this T value) {
            Console.WriteLine(value);
        }

        public static void Print<T>(this T[] items) {
            var text = items.Aggregate(new StringBuilder(), (acc, item) => {
                return acc.Append(" ").Append(item);
            });
            Console.WriteLine(text);
        }
    }

    class SomeClass {
        public SomeClass child;
        public String tag;
        public int? value;

        public SomeClass() {

        }

        public SomeClass(String tag = "", int value = 0, SomeClass child = null) {
            this.tag = tag;
            this.value = value;
            this.child = child;
        }

    }
    
}