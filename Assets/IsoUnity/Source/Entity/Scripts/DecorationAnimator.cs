using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IsoUnity.Entities
{

	[ExecuteInEditMode]
	public class DecorationAnimator : EventedEntityScript {
		
		[System.Serializable]
		public class NameIsoAnimation {
			[SerializeField]
			public string name;
			[SerializeField]
			public IsoAnimation isoAnimation;
		}

		[System.Serializable]
		public class NameIsoDecoration {
			[SerializeField]
			public string name;
			[SerializeField]
			public IsoDecoration isoDecoration;
		}

		[SerializeField]
		public List<NameIsoDecoration> sheets = new List<NameIsoDecoration>();
		[SerializeField]
		public List<NameIsoAnimation> isoAnimations = new List<NameIsoAnimation>();
		[SerializeField]
		private Dictionary<IGameEvent, bool> run = new Dictionary<IGameEvent, bool>();
		[SerializeField]
		private Queue<string> playQueue = new Queue<string>();

		// Animation variables
		//[NonSerialized]
		public int currentFrame;
		//[NonSerialized]
		public IsoAnimation currentAnimation;
		//[NonSerialized]
		public float timeInCurrentFrame;
		//[NonSerialized]
		public IsoDecoration currentSheet;
		//[NonSerialized]
		public bool animationLocked = false;

		[GameEvent]
		public IEnumerator AnimateEntity(string animation, string then = null) 
		{
			// Clear the current animation
			animationLocked = false;
			currentAnimation = null;
			playQueue.Clear();
			
			// Find the animation
			var isoAnimation = isoAnimations.Find(n => n.name.Equals(animation, StringComparison.InvariantCultureIgnoreCase));
			if(isoAnimation == null)
				yield return null;
			
			// Find a sheet
			IsoDecoration sheet = isoAnimation.isoAnimation.overrideSheet;
			if(sheet == null){
				NameIsoDecoration namedSheet = sheets.Find(s =>
				{
					return s.name.Equals(isoAnimation.isoAnimation.sheet, StringComparison.InvariantCultureIgnoreCase);
				});
				if (namedSheet != null) sheet = namedSheet.isoDecoration;
			}
			if(sheet == null)
				yield return null;

			var ge = Current;
			// Stop all other animations
			foreach(var kv in run)
				run[kv.Key] = false;
			
			// Add the new one
			run.Add(ge, true);

			// Perform the animation
			foreach(var frame in isoAnimation.isoAnimation.frames)
			{
				SetFrame(sheet, frame.column);
				yield return new WaitForSeconds(frame.duration);
				if(!run[ge])
					break;
			}

			var desiredRun = run[ge];
			run.Remove(ge);
			if(desiredRun && !string.IsNullOrEmpty(then))
				Play(then);
		}

		public void Play(params string[] animations) 
		{
			currentAnimation = null;
			currentFrame = 0;
			timeInCurrentFrame = 0f;
			animationLocked = false;
			playQueue.Clear();
			foreach(var animation in animations)
			{
				playQueue.Enqueue(animation);
			}
		}

		public override void Update()
		{	
			var timeLeft = Time.deltaTime;
			var desiredFrame = currentFrame;
			var desiredAnimation = currentAnimation;
			if (run.Count == 0 && (desiredAnimation != null || playQueue.Count > 0)) 
			{
				// While there's time left to reproduce
				while (timeLeft > 0 && !animationLocked)
				{
					// Look for the next playable animation
					while (desiredAnimation == null && playQueue.Count > 0)
					{
						var animationToPlay = playQueue.Dequeue();
						var foundAnimation = isoAnimations.Find(n => n.name.Equals(animationToPlay, StringComparison.InvariantCultureIgnoreCase));
						if (foundAnimation == null || foundAnimation.isoAnimation == null || foundAnimation.isoAnimation.frames.Count == 0) 
							continue;

						currentSheet = foundAnimation.isoAnimation.overrideSheet;
						if(currentSheet == null)
						{
							var foundSheet = sheets.Find(n => n.name.Equals(foundAnimation.isoAnimation.sheet, StringComparison.InvariantCultureIgnoreCase));
							if (foundSheet != null) currentSheet = foundSheet.isoDecoration;
						}

						if (currentSheet == null)
							continue;
						
						desiredAnimation = foundAnimation.isoAnimation;
						currentFrame = 0;
						timeInCurrentFrame = 0f;
					} 

					// And if we have animation we play it
					if (desiredAnimation != null)
					{
						var looped = false;
						var previousLoopedTimeLeft = 0f;
						// We move through the frames untill we run out of time or frames
						while(desiredAnimation != null && timeLeft > 0 && desiredFrame < desiredAnimation.frames.Count)
						{
							timeInCurrentFrame += timeLeft;
							if(timeInCurrentFrame > desiredAnimation.frames[desiredFrame].duration) 
							{
								timeLeft = timeInCurrentFrame - desiredAnimation.frames[desiredFrame].duration;
								desiredFrame++;
								timeInCurrentFrame = 0f;
							} else timeLeft = 0;

							if(desiredFrame == desiredAnimation.frames.Count)
							{
								desiredFrame = 0;
								// In case of loop
								if (desiredAnimation.loop)
								{
									// In case we already looped and time didn't decrease, this is an infinite loop
									if(looped && previousLoopedTimeLeft == timeLeft)
									{
										animationLocked = true;
										break;
									}
									previousLoopedTimeLeft = timeLeft;
									looped = true;
								}
								else
								{
									desiredAnimation = null;
								}
							}
						}
					} 
					// But otherwise we exit
					else break;
				}
			}

			if(currentAnimation != desiredAnimation || currentFrame != desiredFrame) 
			{
				currentAnimation = desiredAnimation;
				currentFrame = desiredFrame;
				SetFrame(currentSheet, currentAnimation.frames[currentFrame].column);
			}
		}

		private void SetFrame(IsoDecoration sheet, int column)
		{
			this.Entity.decoration.IsoDec = sheet;
			var tileToSet = column;
			if(sheet.nRows == 4)
				tileToSet += (sheet.nCols * Mover.getDirectionIndex(this.Entity.mover.direction));
			this.Entity.decoration.Tile = tileToSet;
			this.Entity.decoration.updateTextures(false);
			//this.Entity.decoration.adaptate();
		}
	}

}
