# הנחיות להגדרת API Key

## מה זה LOCATIONIQ_API_KEY?
`LOCATIONIQ_API_KEY` הוא משתנה סביבה הנדרש לתחקור כתובות וחישוב מסלולים תוך שימוש ב-LocationIQ API.

## איפה להשיג API Key?
1. בקר בעמוד: https://locationiq.com/
2. הרשמ ל/התחבר לחשבון
3. עבור ל-Account ? API Tokens
4. העתק את ה-API Key שלך

## כיצד להגדיר את משתנה הסביבה

### **דרך 1: launchSettings.json (המומלצת)**
קבצים כבר נוצרו ב:
- `BlTest/Properties/launchSettings.json`
- `DalTest/Properties/launchSettings.json`

פשוט החלף את `pk.YOUR_API_KEY_HERE` בכל קובץ עם ה-API Key שלך:

```json
"environmentVariables": {
  "LOCATIONIQ_API_KEY": "pk.YOUR_ACTUAL_API_KEY"
}
```

**יתרונות:**
? משתנה רק לשימוש מקומי
? לא תעלה ל-GitHub
? כל ריצה ישתמש בהנחיות אלה

---

### **דרך 2: משתנות סביבה בלוח הבקרה (Windows)**

**הגדרה קבועה (דורשת restart):**
```batch
setx LOCATIONIQ_API_KEY "pk.YOUR_API_KEY_HERE"
```

**הגדרה זמנית (רק לחלון הנוכחי):**
```batch
set LOCATIONIQ_API_KEY=pk.YOUR_API_KEY_HERE
```

---

### **דרך 3: Terminal / PowerShell**

**זמנית (רק לשימוש נוכחי):**
```powershell
$env:LOCATIONIQ_API_KEY = "pk.YOUR_API_KEY_HERE"
```

**קבועה (Windows):**
```powershell
[Environment]::SetEnvironmentVariable("LOCATIONIQ_API_KEY", "pk.YOUR_API_KEY_HERE", "User")
```

---

### **דרך 4: קובץ .env (עם NuGet Package)**

אם תרצה להשתמש בקובץ `.env`:

1. התקן package:
```bash
dotnet add package DotNetEnv
```

2. בתוך הקוד (ב-Main):
```csharp
DotNetEnv.DotEnv.Load();
```

3. צור קובץ `.env` בשורש הפרויקט:
```
LOCATIONIQ_API_KEY=pk.YOUR_API_KEY_HERE
```

4. **הוסף ל-.gitignore** כדי שלא תעלה ל-GitHub:
```
.env
```

---

## ?? חשוב - הגנה על ה-API Key

**לעולם אל:**
- ? לא תעלה את ה-API Key ל-GitHub
- ? לא תשתף אותו בציבור
- ? לא תכניס אותו בקוד קשה (hardcoded)

**כן:**
- ? השתמש במשתנות סביבה
- ? הוסף `.env` ו`launchSettings.json` ל-.gitignore
- ? שתף רק את ה-API Key באופן אישי/בטוח

---

## בדיקה שהמשתנה הוגדר

בקוד:
```csharp
string? apiKey = Environment.GetEnvironmentVariable("LOCATIONIQ_API_KEY");
Console.WriteLine(apiKey ?? "לא הוגדר!");
```

ב? PowerShell:
```powershell
$env:LOCATIONIQ_API_KEY
```

ב? CMD:
```batch
echo %LOCATIONIQ_API_KEY%
```

---

## עבור GitHub - .gitignore

ודא שקובץ `.gitignore` שלך מכיל:
```
launchSettings.json
.env
*.env.local
```

כך קבצים אלה לא יעלו ל-GitHub!
