# ProceduralIvyMesh

In this project I tried to reproduce from scratch the "Ivy growing effect" inspired by @TommasoRomano_ amazing work.

##### Next Steps

 - [ ] Build mesh piece by piece (coroutine)
 - [ ] Add some 3D models (e.g., flowers, grass) together with the rest of the mesh
 - [ ] The mesh need to "grow" also on vertical surfaces

**(August 12) - First version** 
In this first version the script works in the following way:

 - Start from a point.
 - Look for other points at a certain distance with a certain degree of randomness.
 - Found all the desired points then a  mesh that goes through those points is created.

![Execution_example](https://media.giphy.com/media/tDHudLlCQr0J2k18D0/giphy.gif)
