using System;
const int threadToSpawn = 5;
const int recordCount = 734999;

Console.WriteLine("INPUT PARAMETERS:");
Console.WriteLine($"Loop {threadToSpawn}");
Console.WriteLine($"Records {recordCount}");
Console.WriteLine();

var (totalRecordperThread, remainders, actualThreadCountToSpawn) = GetRecordsPerThread(threadToSpawn, recordCount);
Console.WriteLine($"Total records per loop is {totalRecordperThread}");
Console.WriteLine($"Total records remaining {remainders}");
Console.WriteLine($"Total expected threads to spawn {threadToSpawn}");
Console.WriteLine($"Total actual threads to spawn {actualThreadCountToSpawn}");


(int, int, int) GetRecordsPerThread(int threadCount, int recordCount)
{
    var actualThreadCountToSpawn = threadCount;
    var totalRecordperThread = recordCount / threadCount;
    var remainders = recordCount % threadCount;

    if (remainders > 0)
        actualThreadCountToSpawn++;

    return (totalRecordperThread, remainders, actualThreadCountToSpawn);
}