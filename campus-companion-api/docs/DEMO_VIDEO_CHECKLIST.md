# Demo video guide — what to click & what to say

Record on a **physical phone** (USB, not only emulator). Speak clearly. Aim for **4–7 minutes**.

When done: upload YouTube **unlisted**, paste the link in the README under **Demo & Rubric Evidence**.

---

## Before you hit Record

1. Start the API on your PC: `.\gradlew.bat run` (repo root).
2. Phone and PC on the **same Wi‑Fi**. In the app, `BASE_URL` must be your PC’s LAN IP + `:8090` (not `10.0.2.2` — that is emulator-only).
3. Seed if needed: `.\gradlew.bat seed`.
4. Install/run the app on the phone from Android Studio.
5. Have a **new test email** ready (or wipe the previous test user) so Create Account works on camera.
6. Optional: open GitHub Actions on the PC so you can flash a green CI tab at the end.

---

## Scene 1 — Intro (10–15 sec)

**On screen:** Phone home screen or app launch / Get Started.

**Say:**
> “This is Campus Companion, Part 2. Android app talks to our Ktor API; Firebase handles Auth; Firestore is only accessed through the API. I’ll show auth, settings, API-backed screens, weather features, and offline sync.”

---

## Scene 2 — Create Account (SSO)

**Clicks:**
1. Open the app → **Get Started** (if on launch).
2. Tap **Create Account**.
3. Fill **Full name**, **Student email**, **Password**.
4. Tap **Create Account**.
5. Wait until you land in the main app (Home).

**Say:**
> “I’m registering with Firebase Email/Password. After sign-up, the app holds an ID token and uses it for authenticated API calls.”

---

## Scene 3 — Sign out → Sign In

**Clicks:**
1. Open **Settings** (profile/gear — however your UI exposes it from Home or the overflow).
2. **Sign out** (if available) *or* force-stop / clear and reopen to the auth screens.
3. Tap **I already have an account** / **Sign In**.
4. Enter the same email + password → **Sign In**.

**Say:**
> “Same account, Sign In path — session restored with Firebase Auth.”

*(Optional biometric — only if already enabled and the phone has a fingerprint/face enrolled.)*  
**Clicks:** Enable biometric in Settings earlier, then reopen app and complete the system prompt.  
**Say:** “Optional biometric unlock on top of Firebase Auth.”

---

## Scene 4 — Settings + language

**Clicks:**
1. Open **Settings**.
2. Point at **name / email** on screen (loaded from `GET /api/users/me`).
3. Change **Preferred language** → **isiZulu** (`zu`) or **Afrikaans** (`af`).
4. Show labels updating (nav / titles).
5. Switch back to **English**.
6. Briefly toggle **notifications** and/or **biometric** if visible.

**Say:**
> “Settings shows the profile from our API. Language preference is saved and the UI switches between English, isiZulu, and Afrikaans.”

---

## Scene 5 — Home + weather (custom feature)

**Clicks:**
1. Bottom nav → **Home**.
2. Tap **Check Weather** (or equivalent).
3. Pick **Rain** (or **Live** if the OpenWeather key works).
4. Scroll **Recommended spaces** — show the list updating.

**Say:**
> “Weather rules drive recommendations. Rain pushes indoor-friendly spaces higher — this is one of our custom business-logic features.”

---

## Scene 6 — Navigate (REST + smart markers)

**Clicks:**
1. Bottom nav → **Navigate**.
2. Wait for markers/list to load.
3. Tap one location → open **detail**.
4. Point out marker colours if visible (recommended / indoor / outdoor / less suitable).
5. **Save** / bookmark the location if the detail screen has that action.
6. Back to the list.

**Say:**
> “Navigate loads campus locations from our Ktor API over REST. Marker colours follow the same weather rules.”

---

## Scene 7 — Events (REST + outdoor banner)

**Clicks:**
1. Bottom nav → **Events**.
2. Scroll the list (loaded from API).
3. Open an **outdoor** event if one shows a weather banner.
4. **Save** that event if possible.
5. Back.

**Say:**
> “Events also come from the API. Outdoor events can show a weather banner when conditions matter.”

---

## Scene 8 — Saved + remove (REST round-trip)

**Clicks:**
1. Bottom nav → **Saved**.
2. Show the location and/or event you just saved.
3. Remove one item (unsave / delete).
4. Confirm it disappears.

**Say:**
> “Saved items are favourites via authenticated API calls — create and delete round-trips.”

---

## Scene 9 — Offline sync (short)

**Clicks:**
1. Pull down the notification shade → turn on **Airplane mode** (or disable Wi‑Fi + mobile data).
2. Go to Navigate or Events → **save** something again.
3. Show the toast: offline / will sync when reconnected.
4. Turn network **back on**.
5. Wait a moment → reopen **Saved** and show the item present (or say WorkManager flushed the queue).

**Say:**
> “If the network drops, we queue the favourite in Room and WorkManager syncs it when we’re online again.”

---

## Scene 10 — Close (CI + team)

**On screen (optional):** Laptop briefly showing GitHub **Actions** green run, or the CI screenshot in the README.

**Say:**
> “Automated tests run in GitHub Actions — backend Gradle tests and Android unit tests plus a debug APK build.  
> Team: Khensani — UI; Khumo — API and Firebase backend; Orearabetse — business rules, integration, and tests.  
> Repo is on GitHub with the README evidence. Thanks.”

Stop recording.

---

## Quick click path (cheat sheet)

```
Get Started → Create Account → submit
→ Settings → Sign out → Sign In
→ Settings → change language (zu/af) → back to English
→ Home → Check Weather → Rain/Live
→ Navigate → open location → Save
→ Events → open event → Save
→ Saved → show items → Remove one
→ Airplane on → Save → toast → Airplane off → Saved
→ (optional) GitHub Actions green
```

---

## If something breaks mid-take

| Problem | Fix |
| :--- | :--- |
| Create Account: API key invalid | Confirm `app/google-services.json` on the phone build; rebuild/reinstall |
| Empty Navigate/Events | API not reachable — check PC IP, firewall, `.\gradlew.bat run`, same Wi‑Fi |
| Profile blank in Settings | Still signed in? API degraded? Check `/health` on the PC |
| Language doesn’t change | Pick another option and wait for activity recreate |
| Offline toast missing | Must fail the network call — Airplane mode before Save |

Do **one clean take** if you can; edit only if you need to cut dead air. Do not fake API data with hardcoded screens.
