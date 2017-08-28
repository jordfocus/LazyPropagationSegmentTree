using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPSegmentTree
{
    class Program
    {
        class LazySegmentTree
        {
            int[] tree;  // To store segment tree
            int[] lazy;  // To store pending updates

            private void ProcessRangeUpdate(int nodeIndex, int currentStart, int currentEnd, int queryStart,
                                 int queryEnd, int difference)
            {
                // check for any lazy updates
                PushUpdatesFromLazyNode(nodeIndex, currentStart, currentEnd);

                // out of range
                if (currentStart > currentEnd || currentStart > queryEnd || currentEnd < queryStart)
                    return;

                // Current segment is fully in range
                if (currentStart >= queryStart && currentEnd <= queryEnd)
                {
                    // Add the difference to current node
                    tree[nodeIndex] += (currentEnd - currentStart + 1) * difference;

                    // leaf node check
                    if (currentStart != currentEnd)
                    {
                        lazy[nodeIndex * 2 + 1] += difference;
                        lazy[nodeIndex * 2 + 2] += difference;
                    }
                    // because the processing segment is fully in range - return
                    return;
                }

                // not completly in range (but overlaps) process children
                int mid = (currentStart + currentEnd) / 2;

                ProcessRangeUpdate(nodeIndex * 2 + 1, currentStart, mid, queryStart, queryEnd, difference);
                ProcessRangeUpdate(nodeIndex * 2 + 2, mid + 1, currentEnd, queryStart, queryEnd, difference);

                //Update this node with the results of the children
                tree[nodeIndex] = tree[nodeIndex * 2 + 1] + tree[nodeIndex * 2 + 2];
            }

            /// <summary>
            /// Checks if there are updates in the current lazy node and push the updates to the segment tree
            /// </summary>
            /// <param name="nodeIndex">Current Segement Tree index</param>
            /// <param name="currentStart">Update range start</param>
            /// <param name="currentEnd">Update range end</param>
            private void PushUpdatesFromLazyNode(int nodeIndex, int currentStart, int currentEnd)
            {
                if (lazy[nodeIndex] != 0)
                {
                    //// update tree[si] with lazy value
                    tree[nodeIndex] += (currentEnd - currentStart + 1) * lazy[nodeIndex];

                    // leaf node check
                    if (currentStart != currentEnd)
                    {
                        // Propagate Values to lazy children...  
                        // we'll update real nodes in the moment when we'll need their values  
                        lazy[nodeIndex * 2 + 1] += lazy[nodeIndex];
                        lazy[nodeIndex * 2 + 2] += lazy[nodeIndex];
                    }

                    // child nodes are updated, re-set lazy parent node
                    lazy[nodeIndex] = 0;
                }
            }

            public void UpdateRange(int arrayLenth, int queryStart, int queryEnd, int difference)
            {
                ProcessRangeUpdate(0, 0, arrayLenth - 1, queryStart, queryEnd, difference);
            }

            // returns sum in range
            private int ProcessSumInRange(int currentStart, int currentEnd, int queryStart, int queryEnd, int nodeIndex)
            {
                // check for any lazy updates
                PushUpdatesFromLazyNode(nodeIndex, currentStart, currentEnd);

                // return if out of range
                if (currentStart > currentEnd || currentStart > queryEnd || currentEnd < queryStart)
                    return 0;

                // this segment completely lies in range
                if (currentStart >= queryStart && currentEnd <= queryEnd)
                {
                    return tree[nodeIndex];
                }

                // part of this segment overlaps with query range
                int mid = (currentStart + currentEnd) / 2;

                return ProcessSumInRange(currentStart, mid, queryStart, queryEnd, 2 * nodeIndex + 1) +
                       ProcessSumInRange(mid + 1, currentEnd, queryStart, queryEnd, 2 * nodeIndex + 2);

            }

            // get sum of elements in range
            public int GetSumInRange(int arrayLength, int queryStart, int queryEnd)
            {
                // Check for erroneous input values
                if (queryStart < 0 || queryEnd > arrayLength - 1 || queryStart > queryEnd)
                {
                    Console.WriteLine("GetSum - Invalid Input");
                    return 0;
                }

                return ProcessSumInRange(0, arrayLength - 1, queryStart, queryEnd, 0);
            }

            private void BuildSegmentTreePart(int[] arr, int currentStart, int currentEnd, int nodeIndex)
            {

                // return if out of range
                if (currentStart > currentEnd)
                    return;

                // only one element in current segment, create leaf node
                if (currentStart == currentEnd)
                {
                    tree[nodeIndex] = arr[currentStart];
                    return;
                }

                // there are more than one element
                int mid = (currentStart + currentEnd) / 2;
                BuildSegmentTreePart(arr, currentStart, mid, nodeIndex * 2 + 1);
                BuildSegmentTreePart(arr, mid + 1, currentEnd, nodeIndex * 2 + 2);

                tree[nodeIndex] = tree[nodeIndex * 2 + 1] + tree[nodeIndex * 2 + 2];
            }

            /* Function to construct segment tree from given array.
               This function allocates memory for segment tree and
               calls constructSTUtil() to fill the allocated memory */
            public void BuildSegmentTree(int[] arr)
            {
                int arrayLength = arr.Length;
                int n = (int)(Math.Ceiling(Math.Log(arrayLength) / Math.Log(2)));
                //Maximum size of segment tree
                int maxTreeSize = 2 * (int)Math.Pow(2, n) - 1;

                tree = new int[maxTreeSize];
                lazy = new int[maxTreeSize];
                // Fill the allocated memory st
                BuildSegmentTreePart(arr, 0, arrayLength - 1, 0);
            }
        }
        static void Main(string[] args)
        {
            int n = 100000;
            int[] arr = new int[n];
            // init an array with 10000 elements
            for(var i=0; i<n; i++)
            {
                arr[i] = 0;
            }
            LazySegmentTree tree = new LazySegmentTree();
            // Build segment tree from given array
            tree.BuildSegmentTree(arr);

            // Print sum of values in array from index 1000 to 7500
            Console.WriteLine("Sum of values in range [1000-7500]  " + tree.GetSumInRange(n, 1000, 7500));

            // Add 100 to all nodes at indexes from 1000 to 7500.
            tree.UpdateRange(n, 1000, 7500, 100);

            // Find sum after the value is updated
            Console.WriteLine("Sum of values in range [1000-7500]  " + tree.GetSumInRange(n, 1000, 7500));

            // Add 1000 to all nodes at indexes from 1000 to 2000.
            tree.UpdateRange(n, 1000, 2000, 1000);

            // Find sum after the value is updated
            Console.WriteLine("Sum of values in range [1000-7500] " + tree.GetSumInRange(n, 1000, 7500));

        }
    }
}
