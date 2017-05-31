using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;


namespace ffp
{
    class Program
    {
        static void Main(string[] args)
        {

            // Console.SetWindowPosition(-10, 0);
            Console.SetWindowSize(150, 65);
            Console.SetBufferSize(150, 2500);


            Console.WriteLine("Display Problem Setup? ");
            bool DisplayProblemSetupText = String.Equals(Console.ReadLine(), "y", StringComparison.CurrentCultureIgnoreCase);

            Console.WriteLine("Display Detailed Results? ");
            bool DisplayDetailedResults = String.Equals(Console.ReadLine(), "y", StringComparison.CurrentCultureIgnoreCase);

            Console.WriteLine("Number Of Iterations?");
            int NumberOfIterations = Convert.ToInt32(Console.ReadLine());

            while (true)
            {

                Console.WriteLine("");
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine("");

                Console.WriteLine("Number of Bins: ");
                int numberOfBins = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Number of Items: ");
                int numberOfItems = Convert.ToInt32(Console.ReadLine());

                Random Random = new Random();

                List<List<FittingResult>> FittingResults = new List<List<FittingResult>>();

                for (int Iteration = 0; Iteration < NumberOfIterations; Iteration++)
                {

                    List<Bin> Bins = new List<Bin>();
                    List<Item> Items = new List<Item>();

                    for (int i = 0; i < numberOfBins; i++)
                    {
                        //Bin Bin = new Bin(i, 1.0);
                        Bin Bin = new Bin(i, Random.NextDouble(), Random.NextDouble());

                        Bins.Add(Bin);
                    }


                    for (int i = 0; i < numberOfItems; i++)
                    {
                        Item Item = new Item(i, Random.NextDouble(), Random.NextDouble());
                        Items.Add(Item);
                    }

                    if (DisplayProblemSetupText) { DisplayProblemSetup(Bins, Items); }


                    List<FitStrategy> FitStrategies = new List<FitStrategy>();

                    //Add all the existing strategies.

                    int counter = 0;
                    int numberofstrategies = 13;
                    foreach (FitStrategy FitStrategyIterator in Enum.GetValues(typeof(FitStrategy)))
                    {
                        if (counter < numberofstrategies)
                        {
                            counter++;
                            FitStrategies.Add(FitStrategyIterator);
                        }
                    }

                    List<FittingResult> FittingResultsPerIteration = FitMultipleStrategies(Bins, Items, FitStrategies);
                    FittingResults.Add(FittingResultsPerIteration);

                    //DisplayMultipleFittingResults(FittingResultsPerIteration);

                    //if (DisplayDetailedResults)
                    //{
                    //    foreach (FittingResult FittingResult in FittingResultsPerIteration)
                    //    {
                    //        DisplaySingleFittingResult(FittingResult);
                    //    }
                    //}


                }

                String[][] TableOfResults = ReturnTableOfStatisticalAnalysisofMultipleIterationsOfMultipleFittingResults(FittingResults);
                int TableWidth = Console.WindowWidth;

                ConsoleWriter.PrintLine(TableWidth);
                ConsoleWriter.PrintRowsOfTable(TableOfResults, TableWidth);

                TextFileWriter.PrintTableToFile(TableOfResults, @"X:\Arch&CivilEng\ResearchProjects\PShepherd\EG-AR1161\Aurimas PhD\Code\ffp\output\ffp.csv"); // Replace with appropriate output path location.

                Console.WriteLine("");

                //Console.WriteLine("Press Any Key To Continue: ");
                // string Command = Console.ReadLine();
            }
        }

        /// <summary>
        /// Use this function when you would like to run multiple fitting strategies and compare their results.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        /// <param name="FitStrategies"></param>
        /// <returns></returns>
        static List<FittingResult> FitMultipleStrategies(List<Bin> Bins, List<Item> Items, List<FitStrategy> FitStrategies)
        {
            List<FittingResult> FittingResults = new List<FittingResult>();

            foreach (FitStrategy FitStrategy in FitStrategies)
            {
                List<Bin> DeepCopyOfBins = CollectionTools.DeepCopyBinList(Bins);
                List<Item> DeepCopyOfItems = CollectionTools.DeepCopyItemList(Items);
                FittingResults.Add(FitSingleStrategy(ref DeepCopyOfBins, ref DeepCopyOfItems, FitStrategy));
            }

            return FittingResults;
        }


        static FittingResult FitSingleStrategy(ref List<Bin> Bins, ref List<Item> Items, FitStrategy FitStrategy)
        {
            Stopwatch StopWatch = new Stopwatch();
            StopWatch.Start();
            switch (FitStrategy)
            {


                case FitStrategy.FirstFit:
                    FirstFit(ref Bins, ref Items);
                    break;
                case FitStrategy.FirstFitSortItemsDecreasingLength:
                    FirstFitDecreasingLength(ref Bins, ref Items);
                    break;
                case FitStrategy.FirstFitSortItemsDecreasingEffect:
                    FirstFitDecreasingEffect(ref Bins, ref Items);
                    break;
                case FitStrategy.FirstFitSortItemsDecreasingLengthSortBinsIncreasingLength:
                    FirstFitDecreasingLengthSortBinsIncreasingLength(ref Bins, ref Items);
                    break;
                case FitStrategy.FirstFitSortItemsDecreasingLengthSortBinsIncreasingResistance:
                    FirstFitDecreasingLengthSortBinsIncreasingResistance(ref Bins, ref Items);
                    break;
                case FitStrategy.FirstFitSortItemsDecreasingEffectSortBinsIncreasingLength:
                    FirstFitDecreasingEffectSortBinsIncreasingLength(ref Bins, ref Items);
                    break;
                case FitStrategy.FirstFitSortItemsDecreasingEffectSortBinsIncreasingResistance:
                    FirstFitDecreasingEffectSortBinsIncreasingResistance(ref Bins, ref Items);
                    break;


                case FitStrategy.BestFitNoSortMinRemainingLength:
                    BestFitMinRemainingLength(ref Bins, ref Items);
                    break;
                case FitStrategy.BestFitNoSortMaxEffectOverResistance:
                    BestFitMaxEffectOverResistance(ref Bins, ref Items);
                    break;
                case FitStrategy.BestFitSortItemsDecreasingLengthMinRemainingLength:
                    BestFitDecreasingLengthMinRemainingLength(ref Bins, ref Items);
                    break;
                case FitStrategy.BestFitSortItemsDecreasingLengthMaxEffectOverResistance:
                    BestFitDecreasingLengthMaxEffectOverResistance(ref Bins, ref Items);
                    break;
                case FitStrategy.BestFitSortItemsDecreasingEffectMinRemainingLength:
                    BestFitDecreasingEffectMinRemainingLength(ref Bins, ref Items);
                    break;
                case FitStrategy.BestFitSortItemsDecreasingEffectMaxEffectOverResistance:
                    BestFitDecreasingEffectMaxEffectOverResistance(ref Bins, ref Items);
                    break;
            }
            StopWatch.Stop();
            return new FittingResult(Bins, Items, FitStrategy, StopWatch.Elapsed); //figureo out how to pass the timeelapsed thingy...
        }



        /// <summary>
        /// First-Fit. Place the item into the first bin which it will fit into, in the order that Items and Bins arrive.
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="Bins"></param>
        static void FirstFit(ref List<Bin> Bins, ref List<Item> Items)
        {

            for (int i = 0; i < Items.Count; i++) // For each Item, in the order they appear:
            {
                Item Item = Items[i];
                Boolean ItemPlaced = false;
                for (int j = 0; j < Bins.Count; j++) // For each Bin, in the order they appear:
                {
                    if (!ItemPlaced)
                    {
                        Bin Bin = Bins[j];
                        if (Item.Length <= Bin.RemainingLength && Item.Effect <= Bin.Resistance)
                        {
                            AddItemToBin(Bin, Item);
                            ItemPlaced = true;
                        }

                    }
                }
                if (ItemPlaced == false)
                {
                    //Console.WriteLine("Failed to fit Item " + i);
                }

            }

        }


        /// <summary>
        /// Performs Best Fit with "Best" Defined as the fit which will result in the lowest remaining length in the Bin being fitted into.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void BestFitMinRemainingLength(ref List<Bin> Bins, ref List<Item> Items)
        {

            double MaximumBinLength = FittingResultComputer.ComputeMaximumBinLength(Bins);

            for (int i = 0; i < Items.Count; i++) // For each Item, in the order they appear:
            {
                Item Item = Items[i];
                Boolean AnyFitFound = false;
                double MinimumWastedLengthRemaining = MaximumBinLength;
                int IndexOfBestFitBin = -1;
                for (int j = 0; j < Bins.Count; j++) // For each Bin, in the order they appear:
                {
                    Bin Bin = Bins[j];
                    if (Item.Length <= Bin.RemainingLength && Item.Effect <= Bin.Resistance)
                    {
                        if (Bin.RemainingLength - Item.Length < MinimumWastedLengthRemaining)
                        {
                            MinimumWastedLengthRemaining = Bin.RemainingLength - Item.Length;
                            IndexOfBestFitBin = j;
                            AnyFitFound = true;
                        }
                    }
                }
                if (AnyFitFound == false)
                {
                    //Console.WriteLine("Failed to fit Item " + i);
                }
                else
                {
                    AddItemToBin(Bins[IndexOfBestFitBin], Item);
                }

            }

        }

        /// <summary>
        /// Performs Best Fit with "Best" defined as the fit which will result in the highest Effect / Resistance.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void BestFitMaxEffectOverResistance(ref List<Bin> Bins, ref List<Item> Items)
        {

            for (int i = 0; i < Items.Count; i++) // For each Item, in the order they appear:
            {
                Item Item = Items[i];
                Boolean AnyFitFound = false;
                double MaximumEffectOverResistance = -1; // Even 0 should be greater than this value, in the case when Effect is 0, but Resistance is non-zero.
                int IndexOfBestFitBin = -1;
                for (int j = 0; j < Bins.Count; j++) // For each Bin, in the order they appear:
                {
                    Bin Bin = Bins[j];
                    if (Item.Length <= Bin.RemainingLength && Item.Effect <= Bin.Resistance)
                    {
                        if ((Item.Effect / Bin.Resistance) > MaximumEffectOverResistance)
                        {
                            MaximumEffectOverResistance = Item.Effect / Bin.Resistance;
                            IndexOfBestFitBin = j;
                            AnyFitFound = true;
                        }
                    }
                }
                if (AnyFitFound == false)
                {
                    //Console.WriteLine("Failed to fit Item " + i);
                }
                else
                {
                    AddItemToBin(Bins[IndexOfBestFitBin], Item);
                }

            }

        }


        /// <summary>
        /// Sort Items in decreasing order of their Length, then perform First Fit.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void FirstFitDecreasingLength(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Length.CompareTo(Item2.Length));
            Items.Reverse();

            FirstFit(ref Bins, ref Items);
        }

        /// <summary>
        /// Sort Items in decreasing order of their Length.
        /// Sort Bins in increasing order of their Length.
        /// Then perform First-Fit.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void FirstFitDecreasingLengthSortBinsIncreasingLength(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Length.CompareTo(Item2.Length));
            Items.Reverse();

            Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));

            FirstFit(ref Bins, ref Items);
        }

        /// <summary>
        /// Sort Items in decreasing order of their Length.
        /// Sort Bins in increasing order of their Resistance.
        /// Then perform First-Fit.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void FirstFitDecreasingLengthSortBinsIncreasingResistance(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Length.CompareTo(Item2.Length));
            Items.Reverse();

            Bins.Sort((Bin1, Bin2) => Bin1.Resistance.CompareTo(Bin2.Resistance));

            FirstFit(ref Bins, ref Items);
        }

        /// <summary>
        /// Sort Items in decreasing order of their Effect.
        /// Sort Bins in increasing order of their Length.
        /// Then perform First-Fit.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void FirstFitDecreasingEffectSortBinsIncreasingLength(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Effect.CompareTo(Item2.Effect));
            Items.Reverse();

            Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));

            FirstFit(ref Bins, ref Items);
        }

        /// <summary>
        /// Sort Items in decreasing order of their Effect.
        /// Sort Bins in increasing order of their Resistance.
        /// Then perform First-Fit.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void FirstFitDecreasingEffectSortBinsIncreasingResistance(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Effect.CompareTo(Item2.Effect));
            Items.Reverse();

            Bins.Sort((Bin1, Bin2) => Bin1.Resistance.CompareTo(Bin2.Resistance));

            FirstFit(ref Bins, ref Items);
        }


        /// <summary>
        /// Sort items in decreasing order of their Effect, then perform First Fit.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void FirstFitDecreasingEffect(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Effect.CompareTo(Item2.Effect));
            Items.Reverse();

            FirstFit(ref Bins, ref Items);
        }

        /// <summary>
        /// Sort Items in Decreasing order of their Length, then perform Best Fit, where "Best" is defined as minimum remaining length in the Bin being fitted into.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void BestFitDecreasingLengthMinRemainingLength(ref List<Bin> Bins, ref List<Item> Items)
        {
            //Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));
            Items.Sort((Item1, Item2) => Item1.Length.CompareTo(Item2.Length));
            Items.Reverse();

            BestFitMinRemainingLength(ref Bins, ref Items);

        }

        /// <summary>
        /// Sort Items in Decreasing order of their Length, then perform Best Fit, where "Best" is defined as maximum Effect / Resistance in the Bin being fitted into.
        /// </summary>
        /// <param name="Bins"></param>
        /// <param name="Items"></param>
        static void BestFitDecreasingLengthMaxEffectOverResistance(ref List<Bin> Bins, ref List<Item> Items)
        {
            //Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));
            Items.Sort((Item1, Item2) => Item1.Length.CompareTo(Item2.Length));
            Items.Reverse();

            BestFitMaxEffectOverResistance(ref Bins, ref Items);

        }

        static void BestFitDecreasingEffectMinRemainingLength(ref List<Bin> Bins, ref List<Item> Items)
        {
            //Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));
            Items.Sort((Item1, Item2) => Item1.Effect.CompareTo(Item2.Effect));
            Items.Reverse();

            BestFitMinRemainingLength(ref Bins, ref Items);

        }

        static void BestFitDecreasingEffectMaxEffectOverResistance(ref List<Bin> Bins, ref List<Item> Items)
        {
            //Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));
            Items.Sort((Item1, Item2) => Item1.Effect.CompareTo(Item2.Effect));
            Items.Reverse();

            BestFitMaxEffectOverResistance(ref Bins, ref Items);

        }

        /*static void FirstFitDecreasingBinsDecreasing(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Length.CompareTo(Item2.Length));
            Items.Reverse();

            Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));
            Bins.Reverse();

            FirstFit(ref Bins, ref Items);
        }*/

        /*static void FirstFitDecreasingBinsIncreasing(ref List<Bin> Bins, ref List<Item> Items)
        {
            Items.Sort((Item1, Item2) => Item1.Length.CompareTo(Item2.Length));
            Items.Reverse();

            //Sort Bins in increasing order of length.
            Bins.Sort((Bin1, Bin2) => Bin1.Length.CompareTo(Bin2.Length));
            

            FirstFit(ref Bins, ref Items);
        }*/


        /// <summary>
        /// Adds an Item to a Bin by:
        /// 1. Adding a reference to the Bin in the .Bin property of the Item.
        /// 2. Adding a reference to the Item in the .Items property of the Bin, and updating the remaining length of the Bin.
        /// </summary>
        /// <param name="Bin"></param>
        /// <param name="Item"></param>
        static void AddItemToBin(Bin Bin, Item Item)
        {
            if (Item.Effect > Bin.Resistance)
            {
                throw new ArgumentException("You are attempting to use a Bin with insufficient Resistance for this Item's Effect.");
            }
            else if (Item.Length > Bin.RemainingLength)
            {
                throw new ArgumentException("You are attempting to place an Item into a Bin which has insufficient Remaining Length.");
            }
            else
            {
                Item.AddToBin(Bin);
                Bin.AddItem(Item);
            }

        }




        static void DisplayProblemSetup(List<Bin> Bins, List<Item> Items)
        {
            Console.WriteLine("");
            Console.WriteLine(">>>Problem Setup:");
            Console.WriteLine("");

            //Console.WriteLine("");
            //Console.WriteLine("Fitting Strategy: " + FittingStrategy);
            //Console.WriteLine("");


            //1. Display Items
            for (int i = 0; i < Items.Count; i++)
            {
                Item Item = Items[i];
                Console.WriteLine("Item " + Item.Index + " -- Length: " + Item.Length + "-- Effect: " + Item.Effect);
            }

            Console.WriteLine("");

            //1. Display Items by index in each Bin.
            for (int i = 0; i < Bins.Count; i++)
            {
                Bin Bin = Bins[i];
                Console.WriteLine("Bin " + Bin.Index + " -- Length: " + Bin.Length + "-- Resistance: " + Bin.Resistance);
            }



        }


        /// <summary>
        /// Prints out the fitting results by:
        /// 1. Listing the Items in each Bin.
        /// 2. Printing the RemainingLength of Each Bin.
        /// 3. Printing the Number of Bins used.
        /// 4. Printing the Total Waste Length (sum of remaining lengths of Bins which have > 0 Items).
        /// 5. Printing the Total Remaining Length (sum of remaining lengths of all Bins). 
        /// </summary>
        /// <param name="Bins"></param>
        static void DisplaySingleFittingResult(FittingResult FittingResult)
        {
            //Console.WriteLine("");
            //Console.WriteLine(">>>Fitting Result:");
            //Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Fit Strategy: " + FittingResult.FitStrategy);
            Console.WriteLine("");


            //Console.WriteLine("Time Taken To Execute: " + StopWatch.Elapsed);
            Console.WriteLine("Number Of Bins Used: " + FittingResult.NumberOfBinsUsed);
            Console.WriteLine("Number Of Items Which Were Not Successfully Fitted: " + FittingResult.NumberOfItemsNotFitted);

            Console.WriteLine("Total Wasted Length: " + FittingResult.TotalWastedLength);
            Console.WriteLine("Total Remaining Length: " + FittingResult.TotalRemainingLength);

            Console.WriteLine("");

            //1. Display Items by index in each Bin.
            for (int i = 0; i < FittingResult.Bins.Count; i++)
            {
                Bin Bin = FittingResult.Bins[i];
                StringBuilder StringBuilder = new StringBuilder();
                for (int j = 0; j < Bin.Items.Count; j++)
                {
                    Item Item = Bin.Items[j];
                    StringBuilder.Append(Item.Index);
                    StringBuilder.Append(",");
                }
                if (StringBuilder.Length >= 1) { StringBuilder.Remove(StringBuilder.Length - 1, 1); }
                Console.WriteLine("Bin " + Bin.Index + " -- Length: " + Bin.Length + "-- Resistance: " + Bin.Resistance + " -- Remaining Length: " + Bin.RemainingLength + " -- Item Indices: " + StringBuilder);
            }




        }

        /// <summary>
        /// Displays a table comparing a list of fitting results.
        /// </summary>
        /// <param name="FittingResults"></param>
        static void DisplayMultipleFittingResults(List<FittingResult> FittingResults)
        {

            int TableWidth = Console.WindowWidth;
            int NumberOfRows = 6; // ??? // number of things we care about displaying... for now let's say...
            int NumberOfColumns = FittingResults.Count + 1;


            String[][] TableStrings = new String[NumberOfRows][];


            for (int i = 0; i < NumberOfRows; i++)
            {
                TableStrings[i] = new String[NumberOfColumns];
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    TableStrings[i][j] = ".";
                }
            }

            //Fill in Row Labels:
            TableStrings[1][0] = "Time Elapsed";
            TableStrings[2][0] = "Number Of Bins Used";
            TableStrings[3][0] = "Number Of Items Not Fitted";
            TableStrings[4][0] = "Total Wasted Length";
            TableStrings[5][0] = "Total Remaining Length";
            //maybe averages and standard deviations and things like that?

            //Fill in Column Labels:
            for (int i = 0; i < FittingResults.Count; i++)
            {
                TableStrings[0][i + 1] = FittingResults[i].FitStrategy.ToString();
            }


            for (int j = 0; j < NumberOfColumns - 1; j++)
            {
                TableStrings[1][j + 1] = FittingResults[j].TimeElapsedInTicks.ToString();
                TableStrings[2][j + 1] = FittingResults[j].NumberOfBinsUsed.ToString();
                TableStrings[3][j + 1] = FittingResults[j].NumberOfItemsNotFitted.ToString();
                TableStrings[4][j + 1] = FittingResults[j].TotalWastedLength.ToString();
                TableStrings[5][j + 1] = FittingResults[j].TotalRemainingLength.ToString();
            }

            ConsoleWriter.PrintLine(TableWidth);
            ConsoleWriter.PrintRowsOfTable(TableStrings, TableWidth);

            TextFileWriter.PrintTableToFile(TableStrings, "");

        }


        static String[][] ReturnTableOfStatisticalAnalysisofMultipleIterationsOfMultipleFittingResults(List<List<FittingResult>> FittingResults)
        {

            //Our List of Lists of FittingResults is stored by iteration, then fit strategy. 
            //Let's transpose it to be stored by fit strategy, then iteration.
            List<List<FittingResult>> FittingResultsByFitStrategy = CollectionTools.Transpose(FittingResults);

            List<StatisticalAnalysisOfFittingResults> StatisticalAnalysesOfFittingResults = new List<StatisticalAnalysisOfFittingResults>();

            //Now construct the statistical analyses for each strategy and add them to our list.
            foreach (List<FittingResult> FittingResultsListByIteration in FittingResultsByFitStrategy)
            {
                StatisticalAnalysisOfFittingResults StatisticalAnalysisOfFittingResults = new StatisticalAnalysisOfFittingResults(FittingResultsListByIteration);
                StatisticalAnalysesOfFittingResults.Add(StatisticalAnalysisOfFittingResults);
            }


            int TableWidth = Console.WindowWidth;
            int NumberOfRows = 7; // ??? // number of things we care about displaying... for now let's say...
            int NumberOfColumns = (StatisticalAnalysesOfFittingResults.Count * 2) + 1; //*2 because we are now storing mean and sd. (2 values for each strategy).

            String[][] TableStrings = new String[NumberOfRows][];

            for (int i = 0; i < NumberOfRows; i++)
            {
                TableStrings[i] = new String[NumberOfColumns];
                for (int j = 0; j < NumberOfColumns; j++)
                {
                    TableStrings[i][j] = ".";
                }
            }

            //Fill in Row Labels:
            //TableStrings[0][0] = "Time Elapsed";
            TableStrings[2][0] = "Time Elapsed";
            TableStrings[3][0] = "Number Of Bins Used";
            TableStrings[4][0] = "Number Of Items Not Fitted";
            TableStrings[5][0] = "Total Wasted Length";
            TableStrings[6][0] = "Total Remaining Length";
            //maybe averages and standard deviations and things like that?

            //Fill in Column Labels:
            for (int i = 0; i < StatisticalAnalysesOfFittingResults.Count; i++)
            {
                TableStrings[0][(i * 2) + 1] = StatisticalAnalysesOfFittingResults[i].FitStrategy.ToString();

            }

            for (int i = 0; i < StatisticalAnalysesOfFittingResults.Count; i++)
            {
                TableStrings[1][(i * 2) + 1] = "Mean";
                TableStrings[1][(i * 2) + 2] = "SD";
            }



            for (int j = 0; j < StatisticalAnalysesOfFittingResults.Count; j++)
            {
                TableStrings[2][(j * 2) + 1] = Math.Round(StatisticalAnalysesOfFittingResults[j].AverageFittingResult.TimeElapsedInTicks, 0).ToString();
                TableStrings[3][(j * 2) + 1] = Math.Round(StatisticalAnalysesOfFittingResults[j].AverageFittingResult.NumberOfBinsUsed, 0).ToString();
                TableStrings[4][(j * 2) + 1] = Math.Round(StatisticalAnalysesOfFittingResults[j].AverageFittingResult.NumberOfItemsNotFitted, 0).ToString();
                TableStrings[5][(j * 2) + 1] = Math.Round(StatisticalAnalysesOfFittingResults[j].AverageFittingResult.TotalWastedLength, 1).ToString();
                TableStrings[6][(j * 2) + 1] = Math.Round(StatisticalAnalysesOfFittingResults[j].AverageFittingResult.TotalRemainingLength, 1).ToString();

                TableStrings[2][(j * 2) + 2] = Math.Round(StatisticalAnalysesOfFittingResults[j].StandardDeviationFittingResult.TimeElapsedInTicks, 0).ToString();
                TableStrings[3][(j * 2) + 2] = Math.Round(StatisticalAnalysesOfFittingResults[j].StandardDeviationFittingResult.NumberOfBinsUsed, 1).ToString();
                TableStrings[4][(j * 2) + 2] = Math.Round(StatisticalAnalysesOfFittingResults[j].StandardDeviationFittingResult.NumberOfItemsNotFitted, 1).ToString();
                TableStrings[5][(j * 2) + 2] = Math.Round(StatisticalAnalysesOfFittingResults[j].StandardDeviationFittingResult.TotalWastedLength, 1).ToString();
                TableStrings[6][(j * 2) + 2] = Math.Round(StatisticalAnalysesOfFittingResults[j].StandardDeviationFittingResult.TotalRemainingLength, 1).ToString();
            }


            return TableStrings;
        }

    }


    static class CollectionTools
    {

        public static List<List<T>> Transpose<T>(List<List<T>> lists)
        {
            var longest = lists.Any() ? lists.Max(l => l.Count) : 0;
            List<List<T>> outer = new List<List<T>>(longest);
            for (int i = 0; i < longest; i++)
                outer.Add(new List<T>(lists.Count));
            for (int j = 0; j < lists.Count; j++)
                for (int i = 0; i < longest; i++)
                    outer[i].Add(lists[j].Count > i ? lists[j][i] : default(T));
            return outer;
        }

        public static List<Bin> DeepCopyBinList(List<Bin> Bins)
        {
            List<Bin> DeepCopyOfBinList = new List<Bin>();

            foreach (Bin OldBin in Bins)
            {
                Bin NewBin = new Bin(OldBin);
                DeepCopyOfBinList.Add(NewBin);
            }


            return DeepCopyOfBinList;
        }

        public static List<Item> DeepCopyItemList(List<Item> Items)
        {
            List<Item> DeepCopyOfItemList = new List<Item>();

            foreach (Item OldItem in Items)
            {
                Item NewItem = new Item(OldItem);
                DeepCopyOfItemList.Add(NewItem);
            }

            return DeepCopyOfItemList;
        }

    }


    enum FitStrategy
    {
        FirstFit,
        FirstFitSortItemsDecreasingLength,
        FirstFitSortItemsDecreasingEffect,
        FirstFitSortItemsDecreasingLengthSortBinsIncreasingLength,
        FirstFitSortItemsDecreasingLengthSortBinsIncreasingResistance,
        FirstFitSortItemsDecreasingEffectSortBinsIncreasingLength,
        FirstFitSortItemsDecreasingEffectSortBinsIncreasingResistance,

        BestFitNoSortMinRemainingLength,
        BestFitNoSortMaxEffectOverResistance,
        BestFitSortItemsDecreasingLengthMinRemainingLength,
        BestFitSortItemsDecreasingLengthMaxEffectOverResistance,
        BestFitSortItemsDecreasingEffectMinRemainingLength,
        BestFitSortItemsDecreasingEffectMaxEffectOverResistance
    };



    class Bin
    {
        public int Index { get; }
        public double Length { get; }
        //BinType Type;
        public double RemainingLength { get; private set; }
        public List<Item> Items { get; private set; }
        public double Resistance { get; private set; }

        public Bin(int Index, double Length, double Resistance)
        {
            this.Index = Index;
            this.Length = Length;
            this.Resistance = Resistance;
            RemainingLength = Length;
            Items = new List<Item>(); // Initialise empty list of Items.
        }

        public Bin(Bin OtherBin)
        {
            this.Index = OtherBin.Index;
            this.Length = OtherBin.Length;
            this.Resistance = OtherBin.Resistance;
            this.RemainingLength = OtherBin.RemainingLength;
            Items = CollectionTools.DeepCopyItemList(OtherBin.Items);
        }

        /// <summary>
        /// Adds an item to a bin, automatically recalculating its remaining length.
        /// </summary>
        public void AddItem(Item Item)
        {
            if (Item.Effect > Resistance)
            {
                throw new ArgumentException("You are attempting to use a Bin with insufficient Resistance for this Item's Effect.");
            }
            else
            {
                Items.Add(Item);
                RemainingLength = RemainingLength - Item.Length;
            }
        }
    }

    class Item
    {
        public int Index { get; }
        public double Length { get; }
        public double Effect { get; }
        public Bin Bin { get; private set; }

        public Item(int Index, double Length, double Effect)
        {
            this.Index = Index;
            this.Length = Length;
            this.Effect = Effect;
        }

        public Item(Item OtherItem)
        {
            Index = OtherItem.Index;
            Length = OtherItem.Length;
            Effect = OtherItem.Effect;
            if (OtherItem.Bin != null) { Bin = new Bin(OtherItem.Bin); } //Because the other item could not have a bin yet.
        }

        public void AddToBin(Bin Bin)
        {
            if (Effect > Bin.Resistance)
            {
                throw new ArgumentException("You are attempting to use a Bin with insufficient Resistance for this Item's Effect.");
            }
            else
            {
                this.Bin = Bin;
            }

        }
    }

    /// <summary>
    /// A class which computes and stores the results of a fitting operation.
    /// </summary>
    class FittingResult
    {
        public FitStrategy FitStrategy { get; }
        public int NumberOfBinsUsed { get; private set; }
        public int NumberOfItemsNotFitted { get; private set; }
        public double TotalWastedLength { get; private set; }
        public double TotalRemainingLength { get; private set; }
        public List<Bin> Bins { get; }
        public List<Item> Items { get; }
        public double TimeElapsedInTicks { get; }

        public FittingResult(List<Bin> Bins, List<Item> Items, FitStrategy FitStrategy, TimeSpan TimeElapsed)
        {
            this.FitStrategy = FitStrategy;
            this.Bins = Bins;
            this.Items = Items;
            this.TimeElapsedInTicks = TimeElapsed.Ticks;
            NumberOfBinsUsed = FittingResultComputer.ComputeNumberOfBinsUsed(Bins);
            NumberOfItemsNotFitted = FittingResultComputer.ComputeNumberOfItemsNotFitted(Items);
            TotalWastedLength = FittingResultComputer.ComputeTotalWastedLength(Bins);
            TotalRemainingLength = FittingResultComputer.ComputeTotalRemainingLength(Bins);

        }



    }

    class AggregatedFittingResult
    {
        public FitStrategy FitStrategy { get; }
        public double NumberOfBinsUsed { get; private set; }
        public double NumberOfItemsNotFitted { get; private set; }
        public double TotalWastedLength { get; private set; }
        public double TotalRemainingLength { get; private set; }
        public double TimeElapsedInTicks { get; }

        public AggregatedFittingResult(FitStrategy FitStrategy, double NumberOfBinsUsed, double NumberOfItemsNotFitted, double TotalWastedLength, double TotalRemainingLength, double TimeElapsedInTicks)
        {
            this.FitStrategy = FitStrategy;
            this.NumberOfBinsUsed = NumberOfBinsUsed;
            this.NumberOfItemsNotFitted = NumberOfItemsNotFitted;
            this.TotalWastedLength = TotalWastedLength;
            this.TotalRemainingLength = TotalRemainingLength;
            this.TimeElapsedInTicks = TimeElapsedInTicks;
        }

    }

    class StatisticalAnalysisOfFittingResults
    {
        public FitStrategy FitStrategy { get; }
        public AggregatedFittingResult AverageFittingResult { get; }
        public AggregatedFittingResult StandardDeviationFittingResult { get; }

        public StatisticalAnalysisOfFittingResults(List<FittingResult> FittingResults)
        {

            #region Compute Average

            double NumberOfBinsUsed = 0;
            double NumberOfItemsNotFitted = 0;
            double TotalWastedLength = 0;
            double TotalRemainingLength = 0;
            double TimeElapsedInTicks = 0;

            FitStrategy = FittingResults[0].FitStrategy;

            foreach (FittingResult FittingResult in FittingResults)
            {
                if (FittingResult.FitStrategy != FitStrategy)
                {
                    throw new ArgumentException("The list of FittingResults contains results which don't have the same FitStrategy... You must have given a badly constructed list of FittingResults.");
                }

                NumberOfBinsUsed += FittingResult.NumberOfBinsUsed;
                NumberOfItemsNotFitted += FittingResult.NumberOfItemsNotFitted;
                TotalWastedLength += FittingResult.TotalWastedLength;
                TotalRemainingLength += FittingResult.TotalRemainingLength;
                TimeElapsedInTicks += FittingResult.TimeElapsedInTicks;
            }


            double AvgNumberOfBinsUsed = NumberOfBinsUsed / FittingResults.Count;
            double AvgNumberOfItemsNotFitted = NumberOfItemsNotFitted / FittingResults.Count;
            double AvgTotalWastedLength = TotalWastedLength / FittingResults.Count;
            double AvgTotalRemainingLength = TotalRemainingLength / FittingResults.Count;
            double AvgTimeElapsedInTicks = TimeElapsedInTicks / FittingResults.Count; // From when working with timespan: http://stackoverflow.com/questions/9993979/timespan-division-by-a-number

            AverageFittingResult = new AggregatedFittingResult(FitStrategy, AvgNumberOfBinsUsed, AvgNumberOfItemsNotFitted, AvgTotalWastedLength, AvgTotalRemainingLength, AvgTimeElapsedInTicks);

            #endregion

            #region Compute Standard Deviation

            double SumOfSquaresOfDeviationsOfNumberOfBinsUsed = 0;
            double SumOfSquaresOfDeviationsOfNumberOfItemsNotFitted = 0;
            double SumOfSquaresOfDeviationsOfTotalWastedLength = 0;
            double SumOfSquaresOfDeviationsOfTotalRemainingLength = 0;
            double SumOfSquaresOfDeviationsOfTimeElapsedInTicks = 0;

            foreach (FittingResult FittingResult in FittingResults)
            {
                SumOfSquaresOfDeviationsOfNumberOfBinsUsed += Math.Pow(FittingResult.NumberOfBinsUsed - AverageFittingResult.NumberOfBinsUsed, 2);
                SumOfSquaresOfDeviationsOfNumberOfItemsNotFitted += Math.Pow(FittingResult.NumberOfItemsNotFitted - AverageFittingResult.NumberOfItemsNotFitted, 2);
                SumOfSquaresOfDeviationsOfTotalWastedLength += Math.Pow(FittingResult.TotalWastedLength - AverageFittingResult.TotalWastedLength, 2);
                SumOfSquaresOfDeviationsOfTotalRemainingLength += Math.Pow(FittingResult.TotalRemainingLength - AverageFittingResult.TotalRemainingLength, 2);
                SumOfSquaresOfDeviationsOfTimeElapsedInTicks += Math.Pow(FittingResult.TimeElapsedInTicks - AverageFittingResult.TimeElapsedInTicks, 2); // don't know how to do this exactly?

            }

            SumOfSquaresOfDeviationsOfNumberOfBinsUsed /= FittingResults.Count;
            SumOfSquaresOfDeviationsOfNumberOfItemsNotFitted /= FittingResults.Count;
            SumOfSquaresOfDeviationsOfTotalWastedLength /= FittingResults.Count;
            SumOfSquaresOfDeviationsOfTotalRemainingLength /= FittingResults.Count;
            SumOfSquaresOfDeviationsOfTimeElapsedInTicks /= FittingResults.Count;

            double SDNumberOfBinsUsed = Math.Pow(SumOfSquaresOfDeviationsOfNumberOfBinsUsed, 0.5);
            double SDNumberOfItemsNotFitted = Math.Pow(SumOfSquaresOfDeviationsOfNumberOfItemsNotFitted, 0.5);
            double SDTotalWastedLength = Math.Pow(SumOfSquaresOfDeviationsOfTotalWastedLength, 0.5);
            double SDTotalRemainingLength = Math.Pow(SumOfSquaresOfDeviationsOfTotalRemainingLength, 0.5);
            double SDTimeElapsedInTicks = Math.Pow(SumOfSquaresOfDeviationsOfTimeElapsedInTicks, 0.5);


            StandardDeviationFittingResult = new AggregatedFittingResult(FitStrategy, SDNumberOfBinsUsed, SDNumberOfItemsNotFitted, SDTotalWastedLength, SDTotalRemainingLength, SDTimeElapsedInTicks);

            #endregion

        }


    }

    static class FittingResultComputer
    {
        public static int ComputeNumberOfBinsUsed(List<Bin> Bins)
        {
            int NumberOfBinsUsed = 0;
            foreach (Bin Bin in Bins)
            {
                if (Bin.Items.Count > 0) { NumberOfBinsUsed++; }
            }
            return NumberOfBinsUsed;
        }

        public static int ComputeNumberOfItemsNotFitted(List<Item> Items)
        {
            int NumberOfItemsNotFitted = 0;
            foreach (Item Item in Items)
            {
                if (Item.Bin == null) { NumberOfItemsNotFitted++; }
            }
            return NumberOfItemsNotFitted;
        }

        public static double ComputeTotalWastedLength(List<Bin> Bins)
        {
            double TotalWastedLength = 0;
            foreach (Bin Bin in Bins)
            {
                if (Bin.Items.Count > 0)
                {
                    TotalWastedLength += Bin.RemainingLength;
                }
            }
            return TotalWastedLength;
        }

        public static double ComputeTotalRemainingLength(List<Bin> Bins)
        {
            double TotalRemainingLength = 0;
            foreach (Bin Bin in Bins)
            {
                TotalRemainingLength += Bin.RemainingLength;
            }
            return TotalRemainingLength;
        }

        public static double ComputeMaximumBinLength(List<Bin> Bins)
        {
            double MaximumBinLength = 0;
            foreach (Bin Bin in Bins)
            {
                if (Bin.Length > MaximumBinLength)
                {
                    MaximumBinLength = Bin.Length;
                }
            }
            return MaximumBinLength;
        }
    }

    public static class TextFileWriter
    {
        public static void PrintTableToFile(string[][] TableStrings, string Path)
        {
            string[] FlattenedTableStrings = new string[TableStrings.Count()];
            for (int i = 0; i < TableStrings.Count(); i++)
            {
                StringBuilder StringBuilder = new StringBuilder();
                for (int j = 0; j < TableStrings[i].Count(); j++)
                {
                    StringBuilder.Append(TableStrings[i][j]);
                    StringBuilder.Append(",");
                }
                FlattenedTableStrings[i] = StringBuilder.ToString();

            }
            System.IO.File.WriteAllLines(Path, FlattenedTableStrings);
        }
    }


    public static class ConsoleWriter
    {
        //static int tableWidth = 77;

        /// <summary>
        /// Prints a rectangular array of data out to the console.
        /// http://stackoverflow.com/questions/856845/how-to-best-way-to-draw-table-in-console-app-c
        /// </summary>
        /// <param name="TableStrings"></param>
        /// <param name="tableWidth"></param>
        public static void PrintRowsOfTable(string[][] TableStrings, int tableWidth)
        {
            //PrintLine(tableWidth);
            for (int i = 0; i < TableStrings.Count(); i++) // assuming every row has same number of elements.
            {
                PrintRow(tableWidth, TableStrings[i]);
            }
        }

        public static void PrintLine(int tableWidth)
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        private static void PrintRow(int tableWidth, params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }


    }
}
