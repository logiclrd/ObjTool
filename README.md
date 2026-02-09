Very simple tool at this point. It has exactly one function which must be activated with
a very specific command-line:

```
ObjTool infile.obj scale 123 outfile.obj
```

The word `scale` must be exactly as written. The operation measures the model along the
primary axes and then scales it so that the longest dimension is the specified length
(123 in this example).
