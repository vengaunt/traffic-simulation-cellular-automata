# TRAFFIC SIMULATION MODEL FOR TRANSPORTATION DESIGN TOOL USING CELLULAR AUTOMATA

# Assigned Students

Civil Engineering:

Oğuzhan Kalkuz 1729304

Beyza Güney 1805249

Ece Güner 1731784

Software Engineering:

Oğuzhan Kalkuz 1729304

Alp Giray Savrum 1803935

Ziya Bahçeci 1805421

# Abstract

It is hard and expensive for a civil engineer to preview traffic and modify the design of the transportation networks in real life. It would be costly and time consuming to rebuild a crossroad over and over. This fact leads to the requirement of a virtual simulation of traffic and transportation networks. This way, without spending resources or workforce, an engineer can get results easier, faster and without cost about his project. To achieve realistic traffic behaviours, cellular automata models can help to identify the entity decisions while simulating the transportation network design. This approach may bring an automation where traffic accidents, avoidances and braking occur. 

# Objectives

To obtain realistic traffic flow

To obtain realistic driver behaviour

To achieve natural accident occurrences, traffic collisions and avoidance

To observe suggested traffic paths and evaluate them

# Preliminary Literature Review

In 2017, Younes Regragui and Najem Moussa have studied urban traffic with roundabouts based on a two dimensional CA model. Regragui and Moussa in 2017 summarize their study in 3 steps; 1. Urban traffic without turning behavior exhibits a transition from free flow congested state as the density exceeds a critical density; 2. The principal cause to the appearance of gridlock is the right turning movement of vehicles, and ultimately; 3. Flow, accidents and waiting time depend on turning rate as well as by the geometry of the urban city. 

  In 2018, Wei Hua et al. have found that in actual traffic incidents, because of the introduction of certain fixed facilities whose examples are construction guardrails, safety cones or toll stations, drivers will slow down to different angles while approaching these roadblocks, therefore resulting in a traffic jam. Meanwhile, in the simulation models that currently exist, such as cellular automata traffic flow models, the vehicle velocity is irrelevant to the cell width (Wei Hua et al., 2018).  Wei Hua et al., in 2018, also have presented a work, where the numerical relationship among the vehicle velocity, the cell deformation and target distance is studied, and a new cellular automata traffic model is proposed to capture spatial variation in lane width. The simulation results of Wei Hua et al. in 2018 show that the effect of the deformation speed limit enforced on the traffic flow by cell deformation is close to actual examined traffic phenomena. Thus, reported by Wei Hua et al. in 2018, the proposed model might be a sensible addition of cellular automata traffic flow models. Although so, due to lack of relevant research and testing, this research has been discontinued.  
	
  In 2019, Liu Yang et al. have done research about the freeway and vehicle characteristics in China. Liu Yang et al. have revised the longitudinal driving rules on normal slopes and uphill based on the NS model in 2019. Liu Yang et al. report that according to their motivation, they classified lane switching into active, inactive and mandatory types, in addition, proposed their motivational expressions in 2019. In accordance with the classification, the asymmetric lane-changing rules on two-lane segments and uphill with a climbing lane were clearly described (Liu Yang et al., 2019). Lane changing was ensured further to be steadier with the longitudinal driving rules and the model was legislated by field data (Liu Yang et al., 2019). The gaps in the heterogeneous traffic flow in-between a standard slope, an uphill without a climbing lane and an uphill with a climbing lane were analyzed by simulation (Liu Yang et al., 2019). Work of Liu Yang et al. consisting of 6 findings, can be summed up as written above. 
	
  In 2020, Jose Raimundo Martinez have published research about Game AI Techniques applied to city simulations. His project is based on two topics which are 1. Implementation of pathfinding algorithm to calculate paths that will be used by vehicles to move throughout the city and 2. An integrated traffic model in order to produce a realistic vehicle movement. Martinez have used pathfinding algorithms such as Dijkstra's Algorithm and A* algorithm to support his paper about pathfinding. Martinez, in 2020, reports that the most common utilization of pathfinding in games is to find a path in-between 2 locations. Although so, pathfinding can be used for many other purposes in video games, as clarified by Martinez in 2020. Martinez further exemplifies the case as pathfinding being used by scout units in Real Time Strategy games to find unexplored waypoints by adding a cost to known territory, heartening units to explore unknown waypoints in 2020. 

Martinez, in his paper, accounts of The Intelligent Driver Model, which is a time-continuous car-following model 	to simulate urban traffic, in 2020. IDM is most likely the simplest complete and accidental free model propagating realistic acceleration and vehicle behaviors in all single-lane traffic situations (Martinez, 2020). Martinez, 2020, exemplifies his claim by (Treiber, Interactive Traffic Simulation), n.d. and (Road Traffic Simulator, n.d.) and claims that those have successfully implemented the IDM for traffic simulation, hence they have been useful references for the integration of the model in his project. 

Martinez highly leans on game engines to support his project. In 2020, Martinez reports that Unreal Engine 4 is a multiplatform component-based game engine thoroughly preferred in the industry of video games, even for the developments of triple-A games, and has gained popularity greatly in the past few years. Martinez adds that it’s being used in other industries such as transportation, architecture and automotive, and not only it has an increasing popularity, but also a rapidly growing community of developers working with the engine, thus there is a wide availability of forums and documentation (2020). Martinez, in 2020, clarified that UE4 is written in C++ but the developers can use “Blueprints”, a visual scripting language, and this system is very flexible. Martinez, in 2020, adds that the engine comes with a tool called “Spline Meshes”, which can be used in his project for curved roads. In short, Martinez claims the possibility of traffic simulation with the help of game engines.

# Methodology
 
  First of all, the simulation space should support transportation network design as it is meant to help transportation engineers and designers. Also, the design will include structures such as grids, Beziér curves as well as custom data structures which are required to specify lane details and driver/vehicle statistics. Beziér curves will be used to hold path data for traversing the map, grid structure for cellular automata application. Unity Engine will be used to render all the simulation graphics and C# is going to be the main language as a result of that. For the first steps of the research, only highway transportations will be allowed.

# References

Liu Yang, Jianlong Zheng, Yang Cheng, Bin Ran, An asymmetric cellular automata model for heterogeneous traffic flow on freeways with a climbing lane ,Physica A: Statistical Mechanics and its Applications, Volume 535, 2019, 122277, ISSN 0378-4371, https://doi.org/10.1016/j.physa.2019.122277.

Hua, Wei & Yue, Yi-Xiang & Wei, Zhenlin & Chen, Jianhua & Wang, Wenrong. A cellular automata traffic flow model with spatial variation in the cell width. Physica A: Statistical Mechanics and its Applications. 2018. 556. 124777. 10.1016/j.physa.2020.124777. 

Younes Regragui, Najem Moussa, A cellular automata model for urban traffic with multiple roundabouts, Chinese Journal of Physics, Volume 56, Issue 3, 2018, Pages 1273-1285,ISSN 0577-9073, https://doi.org/10.1016/j.cjph.2018.02.010.

Martinez, Jose. Game AI techniques applied to city simulations. 202
