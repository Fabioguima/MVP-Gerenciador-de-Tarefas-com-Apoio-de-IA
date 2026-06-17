const state = {
  categories: [],
  tasks: []
};

const $ = (selector) => document.querySelector(selector);

async function api(path, options = {}) {
  const response = await fetch(path, {
    headers: { "Content-Type": "application/json" },
    ...options
  });

  if (!response.ok) {
    const body = await response.json().catch(() => ({}));
    throw new Error(body.error || "Erro na requisicao");
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

function renderCategories() {
  const select = $("#task-category");
  select.innerHTML = '<option value="">Sem categoria</option>';
  for (const category of state.categories) {
    const option = document.createElement("option");
    option.value = category.id;
    option.textContent = category.nome;
    select.appendChild(option);
  }
}

function renderTasks(tasks = state.tasks) {
  const root = $("#tasks");
  root.innerHTML = "";

  if (tasks.length === 0) {
    root.innerHTML = '<article class="task-card">Nenhuma tarefa encontrada.</article>';
    return;
  }

  for (const task of tasks) {
    const category = state.categories.find((item) => item.id === task.categoriaId);
    const card = document.createElement("article");
    card.className = "task-card";
    card.innerHTML = `
      <header>
        <h2>${task.titulo}</h2>
        <button data-complete="${task.id}" ${task.status === "Concluida" ? "disabled" : ""}>Concluir</button>
      </header>
      <p>${task.descricao || "Sem descricao."}</p>
      <div class="meta">
        <span class="badge">${task.status}</span>
        <span class="badge">Prioridade ${task.prioridade}</span>
        <span class="badge">${category ? category.nome : "Sem categoria"}</span>
        <span class="badge">${task.dataLimite || "Sem prazo"}</span>
      </div>
    `;
    root.appendChild(card);
  }
}

async function loadAll() {
  state.categories = await api("/api/categories");
  renderCategories();
  await loadTasks();
}

async function loadTasks() {
  const params = new URLSearchParams();
  if ($("#filter-status").value) params.set("status", $("#filter-status").value);
  if ($("#filter-priority").value) params.set("priority", $("#filter-priority").value);
  state.tasks = await api(`/api/tasks?${params}`);
  renderTasks();
}

$("#category-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  await api("/api/categories", {
    method: "POST",
    body: JSON.stringify({
      nome: $("#category-name").value,
      cor: $("#category-color").value
    })
  });
  event.target.reset();
  await loadAll();
});

$("#task-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  await api("/api/tasks", {
    method: "POST",
    body: JSON.stringify({
      titulo: $("#task-title").value,
      descricao: $("#task-description").value,
      prioridade: $("#task-priority").value,
      categoriaId: $("#task-category").value || null,
      dataLimite: $("#task-date").value || null
    })
  });
  event.target.reset();
  await loadTasks();
});

$("#tasks").addEventListener("click", async (event) => {
  const id = event.target.dataset.complete;
  if (!id) return;
  await api(`/api/tasks/${id}/complete`, { method: "PATCH" });
  await loadTasks();
});

$("#filter-status").addEventListener("change", loadTasks);
$("#filter-priority").addEventListener("change", loadTasks);

$("#ai-prioritize").addEventListener("click", async () => {
  const tasks = await api("/api/ai/prioritize", { method: "POST" });
  renderTasks(tasks);
});

$("#ai-form").addEventListener("submit", async (event) => {
  event.preventDefault();
  const result = await api("/api/ai/suggest", {
    method: "POST",
    body: JSON.stringify({ objetivo: $("#ai-goal").value })
  });
  $("#ai-output").innerHTML = result.sugestoes.map((item) => `<li>${item}</li>`).join("");
});

loadAll().catch((error) => alert(error.message));
