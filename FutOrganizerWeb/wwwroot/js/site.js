function mostrarToast(mensagem, sucesso = true) {
    const toastContainer = document.getElementById('toastContainer');

    if (!toastContainer) return;

    const toast = document.createElement('div');
    const tipo = sucesso ? 'success' : 'danger';

    toast.className = `toast align-items-center text-bg-${tipo} border-0 show fade`;
    toast.setAttribute('role', 'alert');
    toast.setAttribute('aria-live', 'assertive');
    toast.setAttribute('aria-atomic', 'true');

    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">
                ${mensagem}
            </div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Fechar"></button>
        </div>
    `;

    toastContainer.appendChild(toast);

    setTimeout(() => {
        toast.classList.remove('show');
        toast.classList.add('hide');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}



const loginPage = document.getElementById('loginPage');

if (loginPage) {
    const toggleSenha = document.getElementById('toggleSenha');
    const senhaInput = document.getElementById('senha');
    const iconeOlho = document.getElementById('iconeOlho');
    const form = document.getElementById('formLogin');
    const btn = document.getElementById('btnEntrar');
    const btnText = document.getElementById('btnText');
    const btnIcon = document.getElementById('btnIcon');
    const btnSpinner = document.getElementById('btnSpinner');

    // Mostrar/ocultar senha
    toggleSenha.addEventListener('click', () => {
        const mostrar = senhaInput.type === 'password';
        senhaInput.type = mostrar ? 'text' : 'password';
        iconeOlho.classList.toggle('fa-eye', !mostrar);
        iconeOlho.classList.toggle('fa-eye-slash', mostrar);
    });

    // Login
    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const email = document.getElementById('email').value;
        const senha = senhaInput.value;

        // Ativa estado de carregamento
        btn.disabled = true;
        btnIcon.classList.add('d-none');
        btnText.textContent = "Entrando...";
        btnSpinner.classList.remove('d-none');

        fetch('/Login/Logar', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `email=${encodeURIComponent(email)}&senha=${encodeURIComponent(senha)}`
        })
            .then(res => res.json())
            .then(data => {
                if (data.sucesso) {
                    mostrarToast(data.mensagem, true);
                    setTimeout(() => {
                        window.location.href = '/Home';
                    }, 1500);
                } else {
                    mostrarToast(data.mensagem, false);
                    resetarBotao();
                }
            })
            .catch(() => {
                mostrarToast("Erro ao conectar com o servidor.", false);
                resetarBotao();
            });

        function resetarBotao() {
            btn.disabled = false;
            btnText.textContent = "Entrar";
            btnIcon.classList.remove('d-none');
            btnSpinner.classList.add('d-none');
        }
    });
}

var sorteioPage = document.getElementById('sorteioPage');
if (sorteioPage) {
    const teams = [];
    let playerToTransfer = null;
    let editablePlayer = null;

    // Localização
    function salvarLocalizacaoCookie(lat, long) {
        document.cookie = `localizacao=${lat},${long}; path=/; max-age=86400`; // expira em 1 dia
        showToast(`Localização salva: ${lat}, ${long}`, 'success');
    }

    function solicitarLocalizacao() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (pos) => {
                    const { latitude, longitude } = pos.coords;
                    salvarLocalizacaoCookie(latitude, longitude);
                },
                (err) => {
                    showToast('Não foi possível obter a localização.', 'danger');
                    console.error(err);
                }
            );
        } else {
            showToast('Geolocalização não é suportada neste navegador.', 'warning');
        }
    }

    // Solicita localização ao carregar a página
    solicitarLocalizacao();

    function showToast(message, type = 'info') {
        const toastContainer = document.getElementById('toastContainer');
        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-bg-${type} border-0 show`;
        toast.setAttribute('role', 'alert');
        toast.setAttribute('aria-live', 'assertive');
        toast.setAttribute('aria-atomic', 'true');

        toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        `;

        toastContainer.appendChild(toast);
        setTimeout(() => {
            toast.classList.remove('show');
            toast.classList.add('hide');
            setTimeout(() => toast.remove(), 200);
        }, 3000);
    }

    function toggleGoalkeeperInput() {
        const hasFixedGoalkeeper = document.getElementById('hasFixedGoalkeeper').checked;
        const goalkeeperInput = document.getElementById('goalkeeperNames');
        goalkeeperInput.disabled = !hasFixedGoalkeeper;
        goalkeeperInput.value = '';
    }

    function generateTeams() {
        const playersPerTeam = parseInt(document.getElementById('playersPerTeam').value);
        const playerNames = document.getElementById('playerNames').value
            .split('\n')
            .map(name => name.trim())
            .filter(name => name);

        const hasFixedGoalkeeper = document.getElementById('hasFixedGoalkeeper').checked;
        let goalkeeperNames = document.getElementById('goalkeeperNames').value
            .split('\n')
            .map(name => name.trim())
            .filter(name => name);

        if (!playersPerTeam || playersPerTeam <= 0) {
            showToast('Informe a quantidade de jogadores por time.', 'warning');
            return;
        }

        if (playerNames.length === 0) {
            showToast('Adicione jogadores antes de gerar os times!', 'warning');
            return;
        }

        if (hasFixedGoalkeeper && goalkeeperNames.length === 0) {
            showToast('Informe pelo menos um goleiro fixo antes de gerar os times!', 'warning');
            return;
        }

        const totalTeams = Math.ceil(playerNames.length / playersPerTeam);

        teams.length = 0;
        playerNames.sort(() => Math.random() - 0.5);
        goalkeeperNames.sort(() => Math.random() - 0.5);

        if (hasFixedGoalkeeper) {
            while (goalkeeperNames.length < totalTeams) {
                const randomGoalkeeper = goalkeeperNames[Math.floor(Math.random() * goalkeeperNames.length)];
                goalkeeperNames.push(randomGoalkeeper);
            }
        }

        for (let i = 0; i < totalTeams; i++) {
            const playersForTeam = playerNames.splice(0, playersPerTeam);
            let goalkeeper = '';

            if (hasFixedGoalkeeper) {
                goalkeeper = `🧤 ${goalkeeperNames[i % goalkeeperNames.length]}`;
            }

            teams.push({
                name: `Time ${i + 1}`,
                color: `#${Math.floor(Math.random() * 16777215).toString(16)}`,
                players: playersForTeam,
                goalkeeper: goalkeeper
            });
        }

        renderTeams();
        document.getElementById("btnResortear").style.display = "block";
        showToast('Times gerados automaticamente!', 'success');
        criarSorteioAutomaticamente();
    }

    async function criarSorteioAutomaticamente() {
        const nomeSorteio = prompt("Digite o nome do sorteio:");
        if (!nomeSorteio) return showToast("Nome obrigatório para salvar o sorteio.", "warning");

        const localizacao = getCookie("localizacao")?.split(',') || [];
        const latitude = parseFloat(localizacao[0]) || null;
        const longitude = parseFloat(localizacao[1]) || null;

        const request = {
            nomeSorteio,
            local: null,
            latitude,
            longitude,
            times: teams.map(t => ({
                nome: t.name,
                corHex: t.color,
                jogadores: t.players,
                goleiro: t.goalkeeper ? t.goalkeeper.replace("🧤 ", "") : null
            }))
        };

        try {
            const res = await fetch("/Sorteio/Criar", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request)
            });
            const data = await res.json();
            if (data.sucesso) {
                sessionStorage.setItem("sorteioId", data.sorteioId);
                showToast("✅ Sorteio salvo com sucesso!", "success");
            } else {
                showToast("Erro ao salvar sorteio.", "danger");
            }
        } catch (err) {
            console.error(err);
            showToast("Erro na conexão ao salvar sorteio.", "danger");
        }
    }
    document.getElementById('btnResortear').addEventListener('click', resortearTimes);

    async function resortearTimes() {
        const sorteioId = sessionStorage.getItem("sorteioId");
        if (!sorteioId) return showToast("Sorteio não encontrado para resortear.", "danger");

        const playersPerTeam = parseInt(document.getElementById('playersPerTeam').value);
        const hasFixedGoalkeeper = document.getElementById('hasFixedGoalkeeper').checked;

        let playerNames = document.getElementById('playerNames').value
            .split('\n')
            .map(name => name.trim())
            .filter(name => name);

        let goalkeeperNames = document.getElementById('goalkeeperNames').value
            .split('\n')
            .map(name => name.trim())
            .filter(name => name);

        if (!playersPerTeam || playersPerTeam <= 0) {
            return showToast('Quantidade de jogadores inválida.', 'warning');
        }

        const totalTeams = Math.ceil(playerNames.length / playersPerTeam);

        // embaralha várias vezes
        for (let i = 0; i < 3; i++) {
            playerNames.sort(() => Math.random() - 0.5);
            goalkeeperNames.sort(() => Math.random() - 0.5);
        }

        if (hasFixedGoalkeeper) {
            while (goalkeeperNames.length < totalTeams) {
                const randomGoalkeeper = goalkeeperNames[Math.floor(Math.random() * goalkeeperNames.length)];
                goalkeeperNames.push(randomGoalkeeper);
            }
        }

        teams.length = 0;

        for (let i = 0; i < totalTeams; i++) {
            const playersForTeam = playerNames.splice(0, playersPerTeam);
            let goalkeeper = '';

            if (hasFixedGoalkeeper) {
                goalkeeper = `🧤 ${goalkeeperNames[i % goalkeeperNames.length]}`;
            }

            teams.push({
                name: `Time ${i + 1}`,
                color: gerarCorHexAleatoria(),
                players: playersForTeam,
                goalkeeper: goalkeeper
            });
        }

        renderTeams();

        const request = {
            sorteioId,
            times: teams.map(t => ({
                nome: t.name,
                corHex: t.color,
                jogadores: t.players,
                goleiro: t.goalkeeper ? t.goalkeeper.replace("🧤 ", "") : null
            }))
        };

        try {
            const res = await fetch("/Sorteio/Resortear", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(request)
            });
            const data = await res.json();
            if (data.sucesso) {
                showToast("🔁 Times resortados com sucesso!", "success");
            } else {
                showToast("Erro ao resortear os times.", "danger");
            }
        } catch (err) {
            console.error(err);
            showToast("Erro ao conectar com o servidor para resortear.", "danger");
        }
    }

    function gerarCorHexAleatoria() {
        const array = new Uint8Array(3);
        crypto.getRandomValues(array);
        return "#" + Array.from(array).map(b => b.toString(16).padStart(2, '0')).join('');
    }



    function renderTeams() {
        const container = document.getElementById('teamsContainer');
        container.innerHTML = '';

        teams.forEach((team, teamIndex) => {
            const isIncomplete = team.players.length < parseInt(document.getElementById('playersPerTeam').value);
            const cardClass = isIncomplete ? 'team-card incomplete' : 'team-card';

            const goalkeeperHTML = team.goalkeeper ? `
                <h6 class="text-white">Gol:</h6>
                <ul class="list-group mb-3">
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <span>${team.goalkeeper}</span>
                    </li>
                </ul>` : '';

            const linePlayersHTML = team.players.map((player, playerIndex) => `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <span>${playerIndex + 1} - ${renderPlayerName(player, teamIndex, playerIndex)}</span>
                    <div class="btn-group">
                        <button class="btn btn-sm btn-info" onclick="openTransferModal(${teamIndex}, ${playerIndex})">Transferir</button>
                        <button class="btn btn-sm btn-warning" onclick="enableEditing(${teamIndex}, ${playerIndex})">Editar</button>
                    </div>
                </li>`).join('');

            container.innerHTML += `
                <div class="col-md-4">
                    <div class="${cardClass}" style="border-color: ${team.color}; background-color: ${team.color};">
                        <div class="card-header" style="color: white;">
                            ${team.name} (${team.players.length} jogadores)
                        </div>
                        <div class="card-body">
                            ${goalkeeperHTML}
                            <h6 class="text-white">Linha:</h6>
                            <ul class="list-group">
                                ${linePlayersHTML}
                            </ul>
                        </div>
                    </div>
                </div>`;
        });
    }

    function renderPlayerName(player, teamIndex, playerIndex) {
        if (editablePlayer?.teamIndex === teamIndex && editablePlayer?.playerIndex === playerIndex) {
            return `<input type="text" class="form-control player-input" value="${player}" 
                    onblur="updatePlayerName(${teamIndex}, ${playerIndex}, this.value)" 
                    onkeydown="if(event.key === 'Enter') this.blur()" autofocus />`;
        }
        return `<span class="player-name">${player}</span>`;
    }

    function enableEditing(teamIndex, playerIndex) {
        editablePlayer = { teamIndex, playerIndex };
        renderTeams();
    }

    function updatePlayerName(teamIndex, playerIndex, newName) {
        if (newName.trim()) {
            teams[teamIndex].players[playerIndex] = newName.trim();
            showToast('Nome do jogador atualizado com sucesso!', 'success');
        } else {
            showToast('Nome inválido! A edição foi cancelada.', 'warning');
        }
        editablePlayer = null;
        renderTeams();
    }

    function openTransferModal(teamIndex, playerIndex) {
        playerToTransfer = { teamIndex, playerIndex };
        const selectTeam = document.getElementById('selectTeam');
        selectTeam.innerHTML = '<option value="">Selecione um Time</option>';

        teams.forEach((team, index) => {
            if (index !== teamIndex) {
                selectTeam.innerHTML += `<option value="${index}">${team.name}</option>`;
            }
        });

        document.getElementById('teamPlayersList').innerHTML = '';
        selectTeam.addEventListener('change', loadTeamPlayers);
        const transferModal = new bootstrap.Modal(document.getElementById('transferModal'));
        transferModal.show();
    }

    function loadTeamPlayers() {
        const teamIndex = parseInt(document.getElementById('selectTeam').value);
        if (isNaN(teamIndex)) return;

        const team = teams[teamIndex];
        const list = document.getElementById('teamPlayersList');
        list.innerHTML = '<h5 class="text-white">Jogadores do Time Selecionado:</h5>';

        team.players.forEach((player, index) => {
            list.innerHTML += `
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <span class="text-white">${player}</span>
                    <button class="btn btn-sm btn-info" onclick="executeTransfer(${teamIndex}, ${index})">
                        Trocar com ${player}
                    </button>
                </div>`;
        });
    }

    function updatePlayerCount() {
        const playerNames = document.getElementById('playerNames').value
            .split('\n')
            .map(name => name.trim())
            .filter(name => name);

        const count = playerNames.length;
        document.getElementById('playerCount').textContent = `${count} jogador(es) adicionado(s)`;

        const playerList = document.getElementById('playerList');
        playerList.innerHTML = '';

        playerNames.forEach((name, index) => {
            const listItem = document.createElement('li');
            listItem.className = 'list-group-item show';
            listItem.innerHTML = `<span>${index + 1} - ${name}</span>`;
            playerList.appendChild(listItem);
        });
    }

    function executeTransfer(newTeamIndex, newPlayerIndex) {
        if (!playerToTransfer) return;

        const { teamIndex, playerIndex } = playerToTransfer;
        const oldTeam = teams[teamIndex];
        const newTeam = teams[newTeamIndex];

        [oldTeam.players[playerIndex], newTeam.players[newPlayerIndex]] =
            [newTeam.players[newPlayerIndex], oldTeam.players[playerIndex]];

        playerToTransfer = null;
        renderTeams();
        bootstrap.Modal.getInstance(document.getElementById('transferModal')).hide();
        showToast('Jogadores trocados com sucesso!', 'success');
    }

    function getCookie(name) {
        const value = `; ${document.cookie}`;
        const parts = value.split(`; ${name}=`);
        if (parts.length === 2) return parts.pop().split(';').shift();
    }

}


const cronometroPage = document.getElementById('cronometroPage');

if (cronometroPage) {
    let tempoTotal = 0;
    let intervalo = null;

    const minutosInput = document.getElementById('minutos');
    const segundosInput = document.getElementById('segundos');
    const display = document.getElementById('tempoTexto');
    const displayContainer = document.getElementById('displayTempo');
    const btnIniciar = document.getElementById('btnIniciar');
    const btnParar = document.getElementById('btnParar');
    const btnResetar = document.getElementById('btnResetar');

    const alarmeAudio = new Audio('/sounds/mixkit-classic-alarm-995.mp3');

    function tocarAlarme() {
        try {
            alarmeAudio.play().catch(err => console.error('Erro ao tocar som:', err));
        } catch (error) {
            console.error('Erro ao iniciar áudio:', error);
        }

        displayContainer.classList.add('alerta-fim');
    }

    function atualizarDisplay() {
        const minutos = Math.floor(tempoTotal / 60).toString().padStart(2, '0');
        const segundos = (tempoTotal % 60).toString().padStart(2, '0');
        display.textContent = `${minutos}:${segundos}`;
    }

    function iniciarContagem() {
        if (tempoTotal === 0) {
            const min = parseInt(minutosInput.value) || 0;
            const seg = parseInt(segundosInput.value) || 0;
            tempoTotal = min * 60 + seg;
        }

        if (tempoTotal <= 0 || intervalo) return;

        displayContainer.classList.remove('alerta-fim');

        intervalo = setInterval(() => {
            if (tempoTotal <= 0) {
                clearInterval(intervalo);
                intervalo = null;
                tocarAlarme();
                return;
            }
            tempoTotal--;
            atualizarDisplay();
        }, 1000);

        atualizarDisplay();
    }

    function pararContagem() {
        clearInterval(intervalo);
        intervalo = null;
        alarmeAudio.pause();
        alarmeAudio.currentTime = 0;
    }

    function resetarContagem() {
        pararContagem();
        tempoTotal = 0;
        atualizarDisplay();
        minutosInput.value = '';
        segundosInput.value = '';
        displayContainer.classList.remove('alerta-fim');
        alarmeAudio.pause();
        alarmeAudio.currentTime = 0;
    }

    btnIniciar.addEventListener('click', iniciarContagem);
    btnParar.addEventListener('click', pararContagem);
    btnResetar.addEventListener('click', resetarContagem);

    atualizarDisplay(); // inicializa com 00:00
}

var historicoPage = document.getElementById('historicoPage');
if (historicoPage) {
    const container = document.getElementById('historicoContainer');

    fetch('/Partida/ObterHistoricoJson')
        .then(response => {
            if (!response.ok) {
                throw new Error('Erro ao carregar o histórico de partidas.');
            }
            return response.json();
        })
        .then(partidas => {
            container.innerHTML = ''; // limpa os placeholders

            const exibirIncremental = partidas.length > 4;

            partidas.forEach((partida, index) => {
                const delay = exibirIncremental ? index * 300 : 0;

                setTimeout(() => {
                    const col = document.createElement('div');
                    col.className = 'col';

                    const mapaUrl = `https://www.openstreetmap.org/export/embed.html?bbox=${partida.longitude},${partida.latitude},${partida.longitude},${partida.latitude}&layer=mapnik&marker=${partida.latitude},${partida.longitude}`;

                    col.innerHTML = `
                        <div class="card border-0 shadow-lg h-100 rounded-4 animate__animated animate__fadeIn">
                            <div class="card-header bg-gradient bg-dark text-white rounded-top-4 d-flex justify-content-between align-items-center">
                                <h5 class="mb-0">${partida.nome}</h5>
                                <span class="badge bg-secondary">${formatarDataHora(partida.data)}</span>
                            </div>
                            <div class="card-body">
                                <p class="mb-1"><strong>Local:</strong> ${partida.local || "Não informado"}</p>
                                <div class="rounded-3 overflow-hidden mb-3" style="height: 200px;">
                                    <iframe src="${mapaUrl}"
                                            style="width: 100%; height: 100%; border: 0;"
                                            allowfullscreen
                                            loading="lazy"
                                            referrerpolicy="no-referrer-when-downgrade">
                                    </iframe>
                                </div>
                                <a href="/Partida/Detalhes/${partida.partidaId}" class="btn btn-outline-primary w-100">
                                    <i class="fas fa-eye me-2"></i> Ver Detalhes
                                </a>
                            </div>
                        </div>
                    `;

                    container.appendChild(col);
                }, delay);
            });
        })
        .catch(error => {
            container.innerHTML = `
                <div class="col">
                    <div class="alert alert-danger text-center w-100">${error.message}</div>
                </div>`;
        });

    function formatarDataHora(data) {
        const d = new Date(data);
        const dia = d.getDate().toString().padStart(2, '0');
        const mes = (d.getMonth() + 1).toString().padStart(2, '0');
        const ano = d.getFullYear();
        const hora = d.getHours().toString().padStart(2, '0');
        const minuto = d.getMinutes().toString().padStart(2, '0');
        return `${dia}/${mes}/${ano} ${hora}:${minuto}`;
    }
}
