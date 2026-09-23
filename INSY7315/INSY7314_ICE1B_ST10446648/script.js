const STUDENT_NUMBER = "ST10446648";
const XP_PER_MISSION = 40;

const progress = {
  mission8: false,
  mission9: false
};

const studentBtn = document.getElementById("studentBtn");
const messageBtn = document.getElementById("messageBtn");
const messageInput = document.getElementById("messageInput");
const messageOutput = document.getElementById("messageOutput");
const progressFill = document.getElementById("progressFill");
const progressText = document.getElementById("progressText");
const progressBar = document.getElementById("progressBar");
const xpChip = document.getElementById("xpChip");
const statusNote = document.getElementById("statusNote");
const completeBanner = document.getElementById("completeBanner");
const badge8 = document.getElementById("badge8");
const badge9 = document.getElementById("badge9");
const mission8 = document.getElementById("mission8");
const mission9 = document.getElementById("mission9");

function clearedCount() {
  return Number(progress.mission8) + Number(progress.mission9);
}

function updateProgressUI() {
  const cleared = clearedCount();
  const xp = cleared * XP_PER_MISSION;

  progressFill.style.width = (cleared / 2) * 100 + "%";
  progressBar.setAttribute("aria-valuenow", String(cleared));
  progressText.textContent = cleared + " / 2 cleared";
  xpChip.textContent = xp + " XP";
  xpChip.classList.remove("pop");
  void xpChip.offsetWidth;
  xpChip.classList.add("pop");

  if (cleared === 0) {
    statusNote.textContent = "Two missions remain.";
  } else if (cleared === 1) {
    statusNote.textContent = "One mission remaining.";
  } else {
    statusNote.textContent = "Activity B complete — 80 / 80 marks.";
  }

  completeBanner.hidden = cleared !== 2;
}

function clearMission(missionKey, badge, section) {
  if (progress[missionKey]) {
    return;
  }

  progress[missionKey] = true;
  badge.textContent = "Cleared";
  badge.classList.add("cleared");
  section.classList.add("cleared");
  updateProgressUI();
}

// Activity 8: button click pops up the student number
studentBtn.addEventListener("click", function () {
  alert("Student_number: " + STUDENT_NUMBER);
  clearMission("mission8", badge8, mission8);
});

// Activity 9: button click displays what is in the field
messageBtn.addEventListener("click", function () {
  const value = messageInput.value.trim();

  messageOutput.classList.remove("error", "reveal");
  void messageOutput.offsetWidth;

  if (value === "") {
    messageOutput.textContent = "Please type something in the field first.";
    messageOutput.classList.add("error", "reveal");
    return;
  }

  messageOutput.textContent = "You typed: " + value;
  messageOutput.classList.add("reveal");
  clearMission("mission9", badge9, mission9);
});

messageInput.addEventListener("keydown", function (event) {
  if (event.key === "Enter") {
    messageBtn.click();
  }
});

updateProgressUI();
