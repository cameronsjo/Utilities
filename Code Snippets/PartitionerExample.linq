<Query Kind="Program">
  <NuGetReference>Humanizer</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Humanizer</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <AppConfig>
    <Content>
      <configuration>
        <gcAllowVeryLargeObjects enabled="true" />
      </configuration>
    </Content>
  </AppConfig>
</Query>

async Task Main()
{
    var value = 535.Millions();
    
    var myCollection = new List<int>(value);
    myCollection.AddRange(Enumerable.Range(0, value - 1));
    
    Func<int, Task> body = new Func<int, System.Threading.Tasks.Task>(a => Task.FromResult(1));
    
    await Task.WhenAll(from partition in Partitioner.Create(myCollection).GetPartitions(1.Millions())
                       select Task.Run(async delegate
                       {
                           using (partition)
                           {
                               while (partition.MoveNext())
                               {
                                   await body(partition.Current);
                               }
                           }   
                        }));


}

// Define other methods and classes here
