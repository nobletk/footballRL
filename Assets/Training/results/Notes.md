
- **Run03**:
    
    * **Changes**: Added forwardProgress reward along with invertX to reward the 
        agent for pushing the ball forward.

    * **Results**: Slight improvements at the beginning of the training. Then 
    after 500,000 steps the agents starts avoiding the ball, hugging the
    touchlines and running into corners.


- **Run04**: 

    * **Changes**: Added 3m aura reward (+0.0005). Added observation for direction 
    from agent to the ball. Increased observations from 12 to 15.

    * **Results**: Great results deemed useless by incorrect scoring accumulation.

- **Run05**: 

    * **Changes**: Fix the scoring logic to prevent scoring own goals.

    * **Results**: Great results achieved. Entropy.mean is 1.92. 
    Self-play.ELO.mean climbed to 1224.71.

- **Run06**: 

    * **Changes**: Testing more complex Neural Network.

    * **Results**: Great results around 1.5 million steps but results in avoiding
    contact with ball due to big aura around the ball 3.0f compared to field size.

- **Run07**: 

    * **Changes**: Lower the aure around the ball to 0.75f and movement penalty 
    to 0.0001f. Testing with more complex Neural Network.

    * **Results**: 
