
# Game Engines 2 Assignment 2

  

Name: Zan Smirnov

  

Student Number: C15437072



  

# Description of the assignment
The idea behind this assignment was to make a little educational film about wheat. Everything is procedurally generated, including the ground, wind effect, sun effect, the ground, the wheat and the tree in the middle.
  
  

I used this book as reference for LSystems 'The Algorithmic Beauty Of Plants' by Przemyslaw Prusinkiewicz and Aristid Lindenmayer which can be found for free here http://algorithmicbotany.org/papers/abop/abop.pdf

  

# Instructions

Executable is in the '/FF' folder. Or you can open the root folder up in Unity and it will load as a project. I used Unity 2018.3.5, it might not work on 2019.

Or you can view the video at  https://youtu.be/GTPM4BjWq04

# How it works
### L-System
The wheat and the tree in the middle are generated by an [L-Systems](https://en.wikipedia.org/wiki/L-system). 

For example, the wheat stochastic L-System is:

    w: a
    
    (0.90)a -> D1.0Fa
    (0.10)a -> A
    (1.00)A -> MD0.3C
    (0.98)C -> SF[R90&[R45^S][R45)R90-S][R45&R90–S][R45/R90—S]]C
    (0.02)C -> SE
    (1.00)S -> [R30&F[F]][R90/R30&F[F]][R90//R30&F[F]][R90///R30&F[F]]
The L-System goes through 40 iterations. Each rule has a certain probability to happen, meaning all wheats are going to be a little bit different.

 - a is an internode
 - A is the start of the head
 - S is a grain
 - C is a level of grains
 - D is used to set the forward distance i.e. D1.0 sets the forward distance to 1.0
 - R is used to set the rotation angle for example R30 sets the rotation angle to 30 degrees.
 - '[' and ']' is used to save the L-System state by pushing and popping rotation, position, forward value etc. on and off a stack respectively.

The resulting L-System is then drawn using turtle drawing and a line renderer.

### Wind
The wind is a randomly generated force that acts upon every single point in a resulting L-System. The wind is calculated per every wheat plant. It is a random force in a general direction of the global wind that changes every frame and includes things such as wind gusts. 
  
 To make the wheat look like it is affected by the wind, every point inside the L-System is moved by f(x) = x^2 * c where x is how high the point is off of the ground and c is a value that is the max height of the plant divided by 10 and clamped to a maximum value of 10.

The wind speed and wind direction changes randomly in order to make it look realistic.

### Seeds dispering
The seeds are boids with a seek behavior. The seek target is random for each seed but it is a random point in a circle which is down-wind from the wheat. The seeds are also affected by wind.

### Terrain
The terrain is generated using layered perlin noise which is fed into the Unity Terrain object.

# What I am most proud of in the assignment

I am most proud of the wheat L-System and the wind acting upon the wheat. It took a while to figure out how to make an L-System for the wheat and a while for the wind to look realistic.

![Wheat](https://i.imgur.com/DO8XKEr.gif) 


# Video: https://youtu.be/GTPM4BjWq04