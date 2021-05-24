using Godot;
using System;

public class Camera : Godot.Camera
{
	private Vector3 focusPosition;
	private Vector2 angularPosition;
	private float distanceFromFocus = 10.0f;
	
	private float zoomFactor = 0.8f;
	private float horizontalSpeed = 0.01f;
	private float verticalSpeed = 0.01f;
	
	private float movementSpeed = 0.001f;
	
	private bool mouseDown = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		focusPosition = new Vector3(0.0f, 0.0f, 0.0f);
		angularPosition = new Vector2(0.0f, 0.5f*Mathf.Pi);
	}
	
	public override void _Input(InputEvent @event)
	{
		// Mouse in viewport coordinates.
		if (@event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.IsPressed()){
				if (eventMouseButton.ButtonIndex == (int)ButtonList.WheelUp){
					distanceFromFocus *= zoomFactor;
				}
				if (eventMouseButton.ButtonIndex == (int)ButtonList.WheelDown){
					distanceFromFocus /= zoomFactor;
				}
			}
		}
		else if (@event is InputEventMouseMotion eventMouseMotion)
		{
			if (Input.IsMouseButtonPressed(3) && Input.IsKeyPressed((int)KeyList.Shift))
			{
				Vector3 screenNormal = (Translation - focusPosition).Normalized();
				Vector3 screenRight = screenNormal.Cross(Vector3.Up).Normalized();
				Vector3 screenUp = -screenNormal.Cross(screenRight).Normalized();
				float speed = movementSpeed*distanceFromFocus;
				focusPosition += speed*eventMouseMotion.Relative.x*screenRight;
				focusPosition += speed*eventMouseMotion.Relative.y*screenUp;
			} else if (Input.IsMouseButtonPressed(3))
			{
				angularPosition.x -= horizontalSpeed*eventMouseMotion.Relative.x;
				angularPosition.y -= verticalSpeed*eventMouseMotion.Relative.y;
			}
		}
		
		// Implement Gimbal lock:
		double u = angularPosition.x;
		double v = Mathf.Max(Mathf.Min(0.999f*Mathf.Pi, angularPosition.y), 0.001f);
		Vector3 position = distanceFromFocus*(new Vector3((float)(Math.Sin(v)*Math.Sin(u)), (float)Math.Cos(v), (float)(Math.Sin(v)*Math.Cos(u))));
		
		LookAtFromPosition(focusPosition + position, focusPosition, Vector3.Up );
	}
}
