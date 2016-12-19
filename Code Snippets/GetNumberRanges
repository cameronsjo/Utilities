public IEnumerable<string> GetRanges(IEnumerable<int> values)
{
	// Get the distinct sorted values.
	var sorted = values.Distinct()
			   .OrderBy(v => v)
			   .ToArray();

	// We use a hashset, because it's the fastest way to see if a value has already been consumed.
	var used = new HashSet<int>();
	var output = new List<string>();

	for (int i = 0; i < sorted.Length; i++)
	{
		var current = sorted[i];
		
		// If we've already consumed the number then skip over it.
		// A possible alternative is to set the value in the array equal to a "consumed" flag.
		// Although this might add more memory pressure than just use a HashSet.
		// Regardless, my intention for this is for relatively small groups of numbers, so I'm not that perf
		// focused.
		if (used.Contains(current))
    		    continue;	
    		
		var nextIndex = i + 1;
		var min = current;
		var prev = current;
		var max = 0;	
		
		while (nextIndex < sorted.Length)
		{
		    var next = sorted[nextIndex];
			
		    if (next - prev != 1)
		        break;	
				
		    used.Add(next);
		    max = next;
		    prev = next;
		    nextIndex++;
		}	
		
    	used.Add(current);	
	
    	if (max != 0)
	    output.Add($"{min}-{max}");
    	else
    	    output.Add(current.ToString());
    }

    return output;
}
