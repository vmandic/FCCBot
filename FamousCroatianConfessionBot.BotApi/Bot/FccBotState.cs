using Microsoft.Bot.Connector;
using System;

namespace FamousCroatianConfessionBot.Bot
{
  public class FccBotState : IDisposable
	{
		Activity _activity = null;
		StateClient _sc = null;
		BotData _botData = null;

		public FccBotState(Activity activity)
		{
			// Get any saved values
			_activity = activity;
			_sc = activity.GetStateClient();
			_botData = _sc.BotState.GetPrivateConversationData
			(
			  activity.ChannelId,
			  activity.Conversation.Id,
			  activity.From.Id
			);
		}

		public FccBotUserData UserData => _botData.GetProperty<FccBotUserData>(nameof(FccBotUserData)) ?? new FccBotUserData();

		public void SetUserData(FccBotUserData data)
		{
			_botData.SetProperty(nameof(FccBotUserData), data);
			_sc.BotState.SetPrivateConversationData
			(
			  _activity.ChannelId,
			  _activity.Conversation.Id,
			  _activity.From.Id,
			  _botData
			);
		}

		public void Dispose()
		{
			_sc.Dispose();
		}
	}
}