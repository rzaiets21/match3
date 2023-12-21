using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace GGMatch3
{
	public class FlyLineRocketAction : BoardAction
	{
		public struct Params
		{
			public Chip bombChip;

			public Match3Game game;

			public IntVector2 position;

			public ChipType rocketType;

			public bool prelock;

			public bool canUseScale;

			public bool isHavingCarpet;

			public bool isInstant;

			public SwapParams swapParams;

			public Match3Game.InputAffectorExport affectorExport
			{
				get
				{
					if (swapParams == null)
					{
						return null;
					}
					return swapParams.affectorExport;
				}
			}
		}

		private sealed class _003CDoFly_003Ed__5 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public FlyLineRocketAction _003C_003E4__this;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			[DebuggerHidden]
			public _003CDoFly_003Ed__5(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				int num = _003C_003E1__state;
				FlyLineRocketAction flyLineRocketAction = _003C_003E4__this;
				switch (num)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					if (flyLineRocketAction.flyParams.bombChip != null)
					{
						flyLineRocketAction.flyParams.bombChip.RemoveFromGame();
					}
					for (int i = 0; i < flyLineRocketAction.pieceActions.Count; i++)
					{
						FlyRocketPieceAction action = flyLineRocketAction.pieceActions[i];
						flyLineRocketAction.flyParams.game.board.actionManager.AddAction(action);
					}
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					flyLineRocketAction.isAlive = false;
					return false;
				}
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}
		}

		private List<FlyRocketPieceAction> pieceActions = new List<FlyRocketPieceAction>();

		private Params flyParams;

		private IEnumerator flyAnimation;

		public void Init(Params flyParams)
		{
			this.flyParams = flyParams;
			if (flyParams.bombChip != null)
			{
				flyParams.bombChip.RemoveFromSlot();
			}
			FlyRocketPieceAction.Params @params = default(FlyRocketPieceAction.Params);
			@params.game = flyParams.game;
			@params.isHavingCarpet = flyParams.isHavingCarpet;
			@params.canUseScale = flyParams.canUseScale;
			@params.position = flyParams.position;
			@params.prelock = flyParams.prelock;
			@params.isInstant = flyParams.isInstant;
			@params.affectorExport = flyParams.affectorExport;
			if (flyParams.rocketType == ChipType.HorizontalRocket)
			{
				@params.direction = IntVector2.right;
			}
			else
			{
				@params.direction = IntVector2.up;
			}
			FlyRocketPieceAction flyRocketPieceAction = new FlyRocketPieceAction();
			flyRocketPieceAction.Init(@params);
			pieceActions.Add(flyRocketPieceAction);
			@params.direction = -@params.direction;
			@params.ignoreOriginSlot = true;
			flyRocketPieceAction = new FlyRocketPieceAction();
			flyRocketPieceAction.Init(@params);
			pieceActions.Add(flyRocketPieceAction);
			flyParams.game.Play(GGSoundSystem.SFXType.FlyRocket);
		}

		private IEnumerator DoFly()
		{
			return new _003CDoFly_003Ed__5(0)
			{
				_003C_003E4__this = this
			};
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);
			if (flyAnimation == null)
			{
				flyAnimation = DoFly();
			}
			flyAnimation.MoveNext();
		}
	}
}
