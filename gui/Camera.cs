/*
 * Copyright (C) 2021 Freedom of Form Foundation, Inc.
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License, version 2 (GPLv2) as published by the Free Software Foundation.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License, version 2 (GPLv2) for more details.
 * 
 * You should have received a copy of the GNU General Public License, version 2 (GPLv2)
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using Godot;
using System;

namespace FreedomOfFormFoundation.AnatomyRenderer
{
	/// <summary>
	/// 	This Camera class allows the user to rotate, move and zoom the camera
	/// 	using the mouse. Currently, it implements a simplified version of
	/// 	Blender's control scheme.
	/// </summary>
	public class Camera : Godot.Camera
	{
		/// <summary>
		/// 	The target position that the camera is currently looking at.
		/// </summary>
		private Vector3 _focusPosition;
		
		/// <summary>
		/// 	The angular position from which the camera looks at the target.
		/// 	x is the horizontal orientation, y is the vertical orientation.
		/// </summary>
		private Vector2 _angularPosition;
		
		/// <summary>
		/// 	The distance between the camera and the target.
		/// </summary>
		private float _distanceFromFocus = 10.0f;
		
		/// <summary>
		/// 	The speed at which scrolling will move the camera closer or further away from
		/// 	the target.
		/// </summary>
		private float zoomFactor = 1.2f;
		
		/// <summary>
		/// 	The horizontal speed of rotating the camera with the mouse. Also known as the sensitivity.
		/// </summary>
		private float horizontalSpeed = 0.01f;
		
		/// <summary>
		/// 	The vertical speed of rotating the camera with the mouse. Also known as the sensitivity.
		/// </summary>
		private float verticalSpeed = 0.01f;
		
		/// <summary>
		/// 	The speed of moving the camera's target position with the mouse.
		/// </summary>
		private float movementSpeed = 0.001f;
		
		/// <summary>
		/// 	Called when the node enters the scene tree for the first time.
		/// </summary>
		public override void _Ready()
		{
			_focusPosition = new Vector3(0.0f, 0.0f, 0.0f);
			_angularPosition = new Vector2(0.0f, 0.5f*Mathf.Pi);
		}
		
		/// <summary>
		/// 	Called when there is some user input event.
		/// </summary>
		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouseButton eventMouseButton)
			{
				// If there is a scrollwheel event, zoom the camera in or out:
				if (eventMouseButton.IsPressed()){
					if (eventMouseButton.ButtonIndex == (int)ButtonList.WheelUp){
						// Divide by the zoom factor to zoom in:
						_distanceFromFocus /= zoomFactor;
					}
					if (eventMouseButton.ButtonIndex == (int)ButtonList.WheelDown){
						// Multiply by the zoom factor to zoom out:
						_distanceFromFocus *= zoomFactor;
					}
				}
			}
			else if (@event is InputEventMouseMotion eventMouseMotion)
			{
				if (Input.IsMouseButtonPressed(3) && Input.IsKeyPressed((int)KeyList.Shift))
				{
					// If the user moves the cursor while holding the scrollwheel button and holding
					// the Shift key, move the focus (target) position:
					Vector3 screenNormal = (Translation - _focusPosition).Normalized();
					Vector3 screenRight = screenNormal.Cross(Vector3.Up).Normalized();
					Vector3 screenUp = -screenNormal.Cross(screenRight).Normalized();
					float speed = movementSpeed*_distanceFromFocus;
					_focusPosition += speed*eventMouseMotion.Relative.x*screenRight;
					_focusPosition += speed*eventMouseMotion.Relative.y*screenUp;
				} else if (Input.IsMouseButtonPressed(3))
				{
					// If the user moves the cursor while holding the scrollwheel button without
					// modifiers, rotate the camera around the target:
					_angularPosition.x -= horizontalSpeed*eventMouseMotion.Relative.x;
					_angularPosition.y -= verticalSpeed*eventMouseMotion.Relative.y;
				}
			}
			
			// Implement Gimbal lock:
			double u = _angularPosition.x;
			double v = Mathf.Max(Mathf.Min(0.999f*Mathf.Pi, _angularPosition.y), 0.001f);
			
			// Calculate the 3D camera position from the angular position:
			Vector3 position = _distanceFromFocus*(new Vector3((float)(Math.Sin(v)*Math.Sin(u)), (float)Math.Cos(v), (float)(Math.Sin(v)*Math.Cos(u))));
			
			// Set the camera's position and rotation:
			LookAtFromPosition(_focusPosition + position, _focusPosition, Vector3.Up );
		}
	}
}
