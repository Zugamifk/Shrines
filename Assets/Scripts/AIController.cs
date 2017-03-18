using UnityEngine;
using System.Collections;

public class AIController : CharacterController {

    class Meter
    {
        public float value;
        public float currentValue;
        public float fillspeed = 1;
        public bool isSet;
        public Meter(float speed, float defaultValue = 0)
        {
            fillspeed = speed;
            value = defaultValue;
            currentValue = defaultValue;
            isSet = true;
        }

        public void SetValueNow(float to)
        {
            currentValue = to;
            value = to;
            isSet = true;
        }

        public void Update(float dt)
        {
            if (value > currentValue)
            {
                var newVal = currentValue + fillspeed * dt ;
                if(newVal>=value) {
                    currentValue = value;
                    isSet = true;
                } else {
                    currentValue = newVal;
                    isSet = false;
                }
            }
            else if (value < currentValue)
            {
                var newVal = currentValue - fillspeed * dt;
                if (newVal <= value)
                {
                    currentValue = value;
                    isSet = true;
                }
                else
                {
                    currentValue = newVal;
                    isSet = false;
                }
            }
        }
    }

    Meter thinkMeter = new Meter(1,1);
    float move = 1;

    void Update()
    {
        thinkMeter.Update(Time.deltaTime);
        if (thinkMeter.isSet)
        {
            move = Mathf.Sign(Random.value - 0.5f);
            if (Random.value > 0.9f)
            {
                Jump();
            }
            thinkMeter.currentValue = 0;
        }

        Move(move);
    }
}
