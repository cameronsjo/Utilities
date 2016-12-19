public static double StdDev<T>(this IEnumerable<T> list, Func<T, double> values)
{
  // Reference: http://stackoverflow.com/questions/2253874/linq-equivalent-for-standard-deviation
  // Reference: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/ 
  var mean = 0.0;
  var sum = 0.0;
  var stdDev = 0.0;
  var n = 0;
  
  foreach (var value in list.Select(values))
  {
    n++;
    var delta = value - mean;
    mean += delta / n;
    sum += delta * (value - mean);
  }
  
  if (1 < n)
    stdDev = Math.Sqrt(sum / (n - 1));

  return stdDev; 
}
