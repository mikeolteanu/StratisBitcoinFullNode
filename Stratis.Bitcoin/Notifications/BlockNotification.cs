﻿using NBitcoin;
using Stratis.Bitcoin.BlockPulling;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Stratis.Bitcoin.Notifications
{
	/// <summary>
	/// Class used to broadcast about new blocks.
	/// </summary>
	public class BlockNotification
	{
		private readonly ISignals signals;

		public BlockNotification(ConcurrentChain chain, ILookaheadBlockPuller puller, ISignals signals)
		{
			if (chain == null)
				throw new ArgumentNullException("chain");
			if (puller == null)
				throw new ArgumentNullException("puller");
			if (signals == null)
				throw new ArgumentNullException("signals");

			this.Chain = chain;			
			this.Puller = puller;
			this.signals = signals;
		}

		public ILookaheadBlockPuller Puller { get; }

		public ConcurrentChain Chain { get; }

		/// <summary>
		/// Notifies about blocks, starting from block with hash passed as parameter.
		/// </summary>
		/// <param name="startHash">The hash of the block from which to start notifying</param>
		/// <param name="cancellationToken">A cancellation token</param>
		public virtual void Notify(uint256 startHash, CancellationToken cancellationToken)
		{			
			AsyncLoop.Run("block notifier", token =>
			{
				// make sure the chain has been downloaded
				ChainedBlock startBlock = this.Chain.GetBlock(startHash);
				if (startBlock == null)
				{
					return Task.CompletedTask;
				}

				// sets the location of the puller to the latest hash that was broadcasted
				this.Puller.SetLocation(startBlock);

				// send notifications for all the following blocks
				while (true)
				{
					var block = this.Puller.NextBlock(token);

					if (block != null)
					{
						this.signals.Blocks.Broadcast(block);
					}
					else
					{
						break;
					}
				}

				return Task.CompletedTask;
			}, cancellationToken);
		}
	}
}
