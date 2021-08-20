# Deep Dark project

Hello and thank you for checking this project out
This is a simple guide explaining where one can find and test the ai systems i created
1) the scene named prototype contains the WFC ai algorithm aswell as a simple world manager to go with it, it is responsable for generating cities and lights around the world
and in the future will also controll enemy spawns as it has access to all spawn locations available
2) the scene named WFCDebug is very similiar to the prototype but has a debug ui text that show the WFC algorithm working
there is a set of 2 groups of debug uis the one on the write is the current world and the one on the left is the last saved world aka the state of the world before 
a decision was made. In this scene is easy to see the backtracking of the algorithm when it requires to do so
3) the scene named GenAITest is the last usefull scene on the project for this evaluation and it contains the genetic algorithm and its controller
in this scene one enemy will spawn at a time in a random available spawn location.This paired with the use of very simple portfolio AIs allows for testing of 
the genetic algorithm with some ease.


In total in this project one can find:
-WFC algorithm:
		-Supports unity obj allowing for this algorthim to be used in multiple situations
		-Has backtracking preventing it from getting stuck in impossible worlds
		-Has a inspector button that allows the creator to update the WFC outside and saving all the data to a json file
		
-Genetic algorithm :
		- Uses behaviour trees as portoflio behaviours
		- Also has normal genetic traites such as size, speed , etc
		- Uses a Roullette wheel selection system to pick what behaviour to follow
		- Uses a coinflip system when breeding the top performing ais
-Behaviour Trees:
		-All tasks and conditions used in this projects portfolios were created by me and can be found in the Scrips/AI/BehaviourTree folder
	they are also properly identified within Behaviour Designer for easy access for the designers



This project also uses the following outside Assets:
		- Behaviour Designer - Used as a base system for the portfolio behaviours and for easier visualization, no action/Condition has been used
				from their assets.
		- PolyNav - Used to create a navmesh for 2D enviroments that supports polygon colliders aswell as updates to the navmesh in realtime
		- SmartLighting2D - Used to create all the lighting effects used in the project
