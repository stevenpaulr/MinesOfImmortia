using UnityEngine;
using System.Collections;

public class Lineartransformations {

	public delegate float LinearTransformationFunction(float time);

	public static float SmoothStart2(float time){
		return time*time;
	}

	public static float SmoothStart3(float time){
		return time*time*time;
	}

	public static float SmoothStart4(float time){
		return time*time*time*time;
	}

	public static float SmoothStop2(float time){
		float variable = 1.0f-time;
		return (1.0f - variable*variable);
	}

	public static float SmoothStop3(float time){
		float variable = 1.0f-time;
		return (1.0f - variable*variable*variable);
	}

	public static float SmoothStop4(float time){
		float variable = 1.0f-time;
		return (1.0f - variable*variable*variable*variable);
	}

	public static float Mix(LinearTransformationFunction a, LinearTransformationFunction b,float weight, float time){
		return (a(time) + weight * ( b(time) - a(time)));
	}

	public static float Mix2(float weight, float time){
		float start = SmoothStart2(time);
		float stop = SmoothStop2(time);
		return start + weight * (stop - start);
	}

	public static float Mix3(float weight, float time){
		float start = SmoothStart3(time);
		float stop = SmoothStop3(time);
		return start + weight * (stop - start);
	}

	public static float Mix4(float weight, float time){
		float start = SmoothStart4(time);
		float stop = SmoothStop4(time);
		return start + weight * (stop - start);
	}

	public static float BellCurve4(float time){
		return SmoothStop2(time) * SmoothStart2(time);
	}

	public static float BellCurve6(float time){
		return SmoothStop3(time) * SmoothStart3(time);
	}

	public static float BounceClampBottom(float time){
		return Mathf.Abs(time);
	}

	public static float BounceClampTop(float time){
		return (1.0f - Mathf.Abs(1.0f - time));
	}

	public static float BounceClampBottomTop(float time){
		return BounceClampTop(BounceClampBottom(time));
	}

	public static float Scale(LinearTransformationFunction function, float time){
		return function(time) * time;
	}

	public static float ReverseScale(LinearTransformationFunction function, float time){
		return function(time) *( 1.0f - time);
	}

	public static float StepFunction(LinearTransformationFunction function, float numberOfFlickerTimes, float time){

		float stepSize = 0.01f;
		float stepValue = 1/(numberOfFlickerTimes + 1);

		for(int i = 1; i <= numberOfFlickerTimes;i++){
				if((stepValue * i - stepSize) < time && time < (stepValue * i + stepSize))return 0.0f;
		}

		return function(time);
	}
	
}