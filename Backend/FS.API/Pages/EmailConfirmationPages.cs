namespace FS.API.Pages;

internal static class EmailConfirmationPages
{
    public const string ContentType = "text/html; charset=utf-8";

    public static string Success() =>
        BuildPage(
            "Почта подтверждена",
            "Готово",
            "Почта подтверждена",
            "Теперь можно вернуться в приложение и войти в аккаунт.",
            "Открыть приложение",
            true);

    public static string UserNotFoundError() =>
        BuildPage(
            "Не удалось подтвердить почту",
            "Ошибка",
            "Ссылка не сработала",
            "Мы не нашли пользователя для подтверждения почты. Проверьте письмо или запросите новую ссылку.",
            "Вернуться в приложение",
            false);

    private static string BuildPage(
        string title,
        string status,
        string heading,
        string description,
        string action,
        bool isSuccess) =>
        $$"""
          <!doctype html>
          <html lang="ru">
          <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>{{title}}</title>
              <style>
                  :root {
                      color-scheme: light;
                      --page: #f2f2f3;
                      --surface: #ffffff;
                      --surface-soft: #f8f8f9;
                      --ink: #151116;
                      --muted: #7c747b;
                      --primary: #2a0618;
                      --line: #ebe8ea;
                  }

                  * {
                      box-sizing: border-box;
                  }

                  body {
                      margin: 0;
                      min-height: 100vh;
                      display: grid;
                      place-items: center;
                      background: var(--page);
                      color: var(--ink);
                      font-family: Inter, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
                  }

                  .screen {
                      width: min(100%, 390px);
                      min-height: 100vh;
                      display: grid;
                      align-items: center;
                      padding: 18px 14px;
                  }

                  .panel {
                      overflow: hidden;
                      background: var(--surface);
                      border: 1px solid var(--line);
                      border-radius: 28px;
                      box-shadow: 0 22px 70px rgba(42, 6, 24, .08);
                  }

                  .top {
                      display: flex;
                      align-items: center;
                      justify-content: space-between;
                      padding: 18px 18px 8px;
                      font-size: 13px;
                      color: var(--muted);
                  }

                  .icon-button {
                      width: 34px;
                      height: 34px;
                      display: grid;
                      place-items: center;
                      border-radius: 50%;
                      background: var(--surface-soft);
                      color: var(--primary);
                  }

                  .content {
                      padding: 22px 16px 18px;
                  }

                  .status {
                      display: inline-flex;
                      align-items: center;
                      min-height: 32px;
                      padding: 0 14px;
                      border-radius: 18px;
                      background: var(--primary);
                      color: #fff;
                      font-size: 13px;
                      font-weight: 700;
                  }

                  .mark {
                      width: 92px;
                      height: 92px;
                      margin: 26px auto 22px;
                      display: grid;
                      place-items: center;
                      border-radius: 28px;
                      background: {{(isSuccess ? "linear-gradient(135deg, #f8ecef, #ffffff)" : "linear-gradient(135deg, #f5e7eb, #ffffff)")}};
                      border: 1px solid var(--line);
                  }

                  .mark svg {
                      width: 48px;
                      height: 48px;
                      stroke: var(--primary);
                  }

                  h1 {
                      margin: 0;
                      text-align: center;
                      font-size: 26px;
                      line-height: 1.15;
                      font-weight: 800;
                      letter-spacing: 0;
                  }

                  p {
                      max-width: 300px;
                      margin: 12px auto 0;
                      text-align: center;
                      color: var(--muted);
                      font-size: 15px;
                      line-height: 1.45;
                  }

                  .summary {
                      margin-top: 24px;
                      padding: 14px;
                      border-radius: 18px;
                      background: var(--surface-soft);
                      border: 1px solid var(--line);
                  }

                  .row {
                      display: flex;
                      justify-content: space-between;
                      gap: 14px;
                      font-size: 13px;
                  }

                  .row span:first-child {
                      color: var(--muted);
                  }

                  .row span:last-child {
                      max-width: 58%;
                      text-align: right;
                      font-weight: 700;
                  }

                  .action {
                      width: 100%;
                      height: 48px;
                      margin-top: 18px;
                      border: 0;
                      border-radius: 24px;
                      background: var(--primary);
                      color: #fff;
                      font-size: 14px;
                      font-weight: 800;
                      font-family: inherit;
                  }

                  .home {
                      width: 134px;
                      height: 5px;
                      margin: 18px auto 8px;
                      border-radius: 999px;
                      background: #d9d9d9;
                  }

                  @media (min-width: 560px) {
                      .screen {
                          min-height: auto;
                      }

                      .panel {
                          border-radius: 30px;
                      }
                  }
              </style>
          </head>
          <body>
              <main class="screen">
                  <section class="panel" aria-labelledby="email-confirmation-title">
                      <div class="top">
                          <span>FootSteps</span>
                          <span class="icon-button" aria-hidden="true">
                              <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
                                  <path d="M4 12h16"></path>
                                  <path d="M12 4v16"></path>
                              </svg>
                          </span>
                      </div>
                      <div class="content">
                          <span class="status">{{status}}</span>
                          <div class="mark" aria-hidden="true">
                              {{(isSuccess ? SuccessIcon : ErrorIcon)}}
                          </div>
                          <h1 id="email-confirmation-title">{{heading}}</h1>
                          <p>{{description}}</p>
                          <div class="summary">
                              <div class="row">
                                  <span>Статус</span>
                                  <span>{{status}}</span>
                              </div>
                          </div>
                          <button class="action" type="button">{{action}}</button>
                      </div>
                      <div class="home" aria-hidden="true"></div>
                  </section>
              </main>
          </body>
          </html>
          """;

    private const string SuccessIcon =
        """
        <svg viewBox="0 0 24 24" fill="none" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
            <path d="M20 6 9 17l-5-5"></path>
        </svg>
        """;

    private const string ErrorIcon =
        """
        <svg viewBox="0 0 24 24" fill="none" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round">
            <path d="M12 8v5"></path>
            <path d="M12 17h.01"></path>
            <path d="M10.3 4.2 2.5 18a2 2 0 0 0 1.7 3h15.6a2 2 0 0 0 1.7-3L13.7 4.2a2 2 0 0 0-3.4 0Z"></path>
        </svg>
        """;
}
