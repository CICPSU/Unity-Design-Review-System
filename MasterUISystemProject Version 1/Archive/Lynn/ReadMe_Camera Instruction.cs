 /* 
 * 
 * Character Control Instruction
 * Press and hold:
 *       W/S key to move forward and backward
 * 	     A/D key to rotate left and right
 * 		(If A/D does not work properly, go to Edit -- Project Settings -- Input, and then find the Jump at the bottom,
 * 			change its name to Rotate, and type in "left" in Negative Button, "right" in Positive Button, "a" in Alt Neg, "d" in Alt Pos, Set Gravity to 3 and Dead = 0.001)
 * 		SPACE to jump
 * 		Q/E to move sidewards
 * 
 * Press and hold:
 * 		Left Mouse Button: rotate the camera around the character.
 * 		Right Mouse Button: rotate the camera and the characeter at the same time.
 * 		Scroll Wheel: the character will move forward and, if you move your mouse, rotate with the camera.
 * 
 * Functions:
 * 		+/-: increase or decrease speed
 * 		g: toggle gravity on or off. When gravity is off, use up and down arrow to elevate or descend
 * 		c: toggle camera between ICon lab camera and single camera
 * Joystick control: [not available in this version]
 * 		Left analog stick moves forward/backward, and leftward/rightward
 * 		Directional Pad moves forward/backward, and turns to left and right
 * 		Right analog stick controls the camera look direction. 
 * 			Holding left trigger while move the right thumb stick will orbit the camera around the character 
 * 			Holding right trigger while move the right thumb stick will rotate the character with the camera 
 * 
 * 
 * **** THIS WOW STYLE THIRD PERSON CAMERA IS DEVELOPED BY THE CIC RESEARCH GROUP AT PENN STATE BASED ON 3D BUZZ TP_CAMERA TUTORIALS.
 ****** Questions and suggestions please contact: Yifan Liu ivenaieseccqu@gmail.com
 *
 *Version History:
 *
 *Joystick integration version: 
 *		Speed was fixed, Joystick fully compatible, avatar scale scaled down for indoor walk. Updated on Dec 3rd, 2012 by Yifan
 *
 *Integrated FP version: Updated Jan 23rd by Yifan
 *		Shape slimmered from 0.7 to 0.5.
 *		speed optimized again
 *		rotation in FP view bug fixed.
 *
 *SelfConfig version: Updated March. 6th by Yifan
 *		Now no set up in input manager is needed anymore!!!
 *		Joystick function has been removed due to the rare usage.
 *		Added a TP_InputManager class that is attached to Lynn to allow users to customize control.
 *		Added ability to toggle camera between normal camera and ICon Lab camera
 *		Added the "Cameras" folder that stores all the cameras
 *		Added reference to the prefab in the awake of TP_Camera
 *		Continue SelfConfig update on Mar. 24 by Yifan:
 *		Moved all the key input function into the TP_Camera GetPlayerInput function for easy management
 *		!!Added function to allow Lynn to ignore collisions when gravity is off
 *
 *KNOWN BUGS:
 *1. camera shaking when colliding with walls
 *2. left rotate and right rotate is strange when in first person view
 *
 */

/* THIS IS THE INSTRUCTION FOR OLDER VERSION. STORED HERE ONLY FOR FUTUER REFERENCE.
 * Before using this camera, go to Edit -- Project Settings -- Input, Set the size of the Axes to 38
 * Set up the input manager according to following intructions: (incorrect setting will make the controller not workable)
 * 		Find the "Vertical" right above the "Fire1", duplicate it(Ctrl + D), uncheck the invert, and change its Axis to "7th axis(Joystick)"
 * 		Find the "Jump" at the bottom,change its name to Rotate, and type in "left" in Negative Button, "right" in Positive Button, "a" in Alt Neg, "d" in Alt Pos, 
 * 		Set Gravity to 3 and Dead = 0.001)
 * 		Duplicate the "Rotate" you just created, set Gravity to 0, Dead to 0.19, Type to "Joystick Axis", Axis to "6th axis"
 * 		Find a "Jump" from the bottom, rename it "JoystickTrigger", clear all fields above Gravity, set Dead to 0.19, Type to "Joystick Axis", and Axis to "3rd Axis"
 * 		Find a "Jump" from the bottom, rename it "JoystickLookX", clear all fields above Gravity, set Dead to 0.19, Type to "Joystick Axis", and Axis to "4th Axis"
 * 		Find a "Jump" from the bottom, rename it "JoystickLookY", clear all fields above Gravity, set Dead to 0.19, Type to "Joystick Axis", and Axis to "5th Axis"
 * 		Find the "Horizontal", set its Negative Button to "q", and Positive Button to "e", delete its Alt Negative, and Alt Positive field
*/