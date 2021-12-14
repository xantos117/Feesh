# Feesh

An exploration of flocking behaviors in a predator-prey dynamic. Simulates fish schooling behavior in response to predators by using low vision range and a panic state for the fish. Predators follow simple hunting rules.

BUILT AND TESTED IN UNITY 2020.3.18f1

### OVERVIEW
---
There are 5 main components to the project:
1. The `FeeshSpawner` class handles spawning in `Feesh` and `Predator` instances, updating them, maintaining a balance of weights on parameters, and maintaining slider value synchronization
2. The `FeeshSettings` class maintains useful constants shared between all `Mover` instances
3. The `Mover` class serves as a base class for our moving actors, which use many of the same rules. The usages for these functions occur in specific subclasses, so the `Mover` class mainly contains functions to generate steering forces
4. The `Predator` class uses 2 update behaviors to guide its movement: the random walk, and Seek to the nearest `Feesh`. 
5. The `Feesh` class has the most complex set of states. It can either be a leader `Feesh`, which ignores the environment and follows a set path, or be a flocking agent in 1 of 2 states, "Normal" or "Panicked". A Normal `Feesh` follows the leader, maintains separation, attempts to cluster and aligns with neighbors. A Panicked `Feesh` flees predators, attempts to travel directly away from them, attempts to swim opposite their direction, and attempts to clump with other `Feesh`. 
### TO RUN
---
* Clone repository
* Add project to Unity Hub
* Open project and run

Alternatively, see the compiled version at xantos117.github.io


### SOURCES
---
* "Fish low poly" (https://skfb.ly/6BCSE) by RacKrab is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
https://sketchfab.com/3d-models/fish-low-poly-bf36d74f863f492a9f3a72dbae4e9a29

* "Koi Fish" (https://skfb.ly/6WUUD) by RunemarkStudio is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
https://sketchfab.com/3d-models/koi-fish-8ffded4f28514e439ea0a26d28c1852a

* "Free Low Poly Fish Pack" (https://skfb.ly/6SSOD) by LuisFelipe is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
https://sketchfab.com/3d-models/free-low-poly-fish-pack-84bd29ae81ce46388e5f8935964bc4b4

* "Low Poly Shark" (https://skfb.ly/6GNN6) by AlienDev is licensed under Creative Commons Attribution (http://creativecommons.org/licenses/by/4.0/).
https://sketchfab.com/3d-models/low-poly-shark-58eddd6fbc2448c38efd1e3df3d0f342

* Camera movement:
https://dev.to/matthewodle/simple-3d-camera-movement-1lcj

https://natureofcode.com/book/chapter-6-autonomous-agents/

Vabø, R., & Nøttestad, L. (1997). An individual based model of fish school reactions: Predicting antipredator behaviour as observed in nature. Fisheries Oceanography, 6(3), 155–171. https://doi.org/10.1046/j.1365-2419.1997.00037.x

Tu, X., & Terzopoulos, D. (1994). Artificial fishes. Proceedings of the 21st Annual Conference on Computer Graphics and Interactive Techniques - SIGGRAPH ’94, 43–50. https://doi.org/10.1145/192161.192170

Reynolds, C. W. (1987). Flocks, herds, and schools: A distributed behavioral model. Proceedings of the 14th Annual Conference on Computer Graphics and Interactive Techniques, SIGGRAPH 1987, 21(4), 25–34. https://doi.org/10.1145/37401.37406

Coding Adventure: Boids - Sebastian Lague
