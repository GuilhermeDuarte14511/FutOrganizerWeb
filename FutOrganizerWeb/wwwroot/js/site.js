﻿document.addEventListener('DOMContentLoaded', function () {

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

        function salvarLocalizacaoCookie(lat, long) {
            document.cookie = `localizacao=${lat},${long}; path=/; max-age=86400`;
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
                    color: gerarCorHexAleatoria(),
                    players: playersForTeam,
                    goalkeeper: goalkeeper
                });
            }

            renderTeams();
            document.getElementById("btnResortear").style.display = "block";
            showToast('Times gerados automaticamente!', 'success');
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
                    <button class="btn btn-sm btn-info transfer-btn" data-team="${teamIndex}" data-player="${playerIndex}">Transferir</button>
                    <button class="btn btn-sm btn-warning edit-btn" data-team="${teamIndex}" data-player="${playerIndex}">Editar</button>
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

            // Reanexa os eventos após render
            document.querySelectorAll('.edit-btn').forEach(btn =>
                btn.addEventListener('click', e => enableEditing(+btn.dataset.team, +btn.dataset.player))
            );
            document.querySelectorAll('.transfer-btn').forEach(btn =>
                btn.addEventListener('click', e => openTransferModal(+btn.dataset.team, +btn.dataset.player))
            );
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
            const transferModal = new bootstrap.Modal(document.getElementById('transferModal'));
            transferModal.show();
        }

        document.getElementById('selectTeam')?.addEventListener('change', loadTeamPlayers);

        function loadTeamPlayers() {
            const teamIndex = parseInt(document.getElementById('selectTeam').value);
            if (isNaN(teamIndex)) return;

            const team = teams[teamIndex];
            const list = document.getElementById('teamPlayersList');
            list.innerHTML = '<h5 class="text-white">Jogadores do Time Selecionado:</h5>';

            team.players.forEach((player, index) => {
                const button = document.createElement('button');
                button.className = 'btn btn-sm btn-info';
                button.textContent = `Trocar com ${player}`;
                button.addEventListener('click', () => executeTransfer(teamIndex, index));

                const wrapper = document.createElement('div');
                wrapper.className = 'd-flex justify-content-between align-items-center mb-2';
                wrapper.innerHTML = `<span class="text-white">${player}</span>`;
                wrapper.appendChild(button);

                list.appendChild(wrapper);
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

        function gerarCorHexAleatoria() {
            const array = new Uint8Array(3);
            crypto.getRandomValues(array);
            return "#" + Array.from(array).map(b => b.toString(16).padStart(2, '0')).join('');
        }

        function getCookie(name) {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            if (parts.length === 2) return parts.pop().split(';').shift();
        }

        let typingTimeout;
        let isTyping = false;
        let pararAtualizacaoLobby = false;

        const textarea = document.getElementById('playerNames');
        const count = document.getElementById('playerCount');
        const list = document.getElementById('playerList');

        textarea?.addEventListener('input', () => {
            isTyping = true;
            clearTimeout(typingTimeout);

            typingTimeout = setTimeout(() => {
                isTyping = false;
            }, 5000);
        });

        if (isLobbyMode()) {
            setInterval(async () => {
                if (isTyping || pararAtualizacaoLobby) return;

                const codigo = new URLSearchParams(window.location.search).get('codigo');
                if (!codigo) return;

                try {
                    const res = await fetch(`/Lobby/Jogadores?codigo=${codigo}`);
                    const jogadoresServidor = await res.json();

                    const jogadoresAtuais = textarea.value
                        .split('\n')
                        .map(j => j.trim())
                        .filter(j => j);

                    const novos = jogadoresServidor.filter(j => !jogadoresAtuais.includes(j));
                    const todos = [...jogadoresAtuais, ...novos];

                    textarea.value = todos.join('\n');

                    count.textContent = `${todos.length} jogador(es) adicionado(s)`;
                    list.innerHTML = '';
                    todos.forEach((j, i) => {
                        const li = document.createElement('li');
                        li.className = 'list-group-item show';
                        li.innerHTML = `<span>${i + 1} - ${j}</span>`;
                        list.appendChild(li);
                    });
                } catch (err) {
                    console.error('Erro ao buscar jogadores do lobby:', err);
                }
            }, 3000);
        }

        function isLobbyMode() {
            const urlParams = new URLSearchParams(window.location.search);
            return urlParams.has('codigo');
        }



        document.getElementById('playerNames')?.addEventListener('input', updatePlayerCount);
        // Gerar ao apertar Enter no input
        document.getElementById('playersPerTeam')?.addEventListener('keydown', e => {
            if (e.key === 'Enter') generateTeams();
        });

        // Gerar ao clicar no botão
        document.getElementById('btnGerarTimes')?.addEventListener('click', generateTeams);
        document.getElementById('btnResortear')?.addEventListener('click', generateTeams);
        document.getElementById('hasFixedGoalkeeper')?.addEventListener('change', toggleGoalkeeperInput);
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

    const formCriarSala = document.getElementById('formCriarSala');
    if (formCriarSala) {
        formCriarSala.addEventListener('submit', (e) => {
            const localizacao = getCookie("localizacao")?.split(',') || [];
            const latitude = localizacao[0] ? parseFloat(localizacao[0]).toFixed(7) : '';
            const longitude = localizacao[1] ? parseFloat(localizacao[1]).toFixed(7) : '';

            const inputLatitude = document.createElement("input");
            inputLatitude.type = "hidden";
            inputLatitude.name = "Latitude";
            inputLatitude.value = latitude;

            const inputLongitude = document.createElement("input");
            inputLongitude.type = "hidden";
            inputLongitude.name = "Longitude";
            inputLongitude.value = longitude;

            const inputDataHora = document.createElement("input");
            inputDataHora.type = "hidden";
            inputDataHora.name = "DataHora";
            inputDataHora.value = new Date().toISOString();

            formCriarSala.appendChild(inputLatitude);
            formCriarSala.appendChild(inputLongitude);
            formCriarSala.appendChild(inputDataHora);
        });


        function getCookie(name) {
            const value = `; ${document.cookie}`;
            const parts = value.split(`; ${name}=`);
            if (parts.length === 2) return parts.pop().split(';').shift();
        }
    }



    var criarSalaPage = document.getElementById('criarSalaPage');
    if (criarSalaPage) {
        function copiarLink() {
            const input = document.getElementById('linkCompartilhavel');
            input.select();
            input.setSelectionRange(0, 99999);
            document.execCommand("copy");

            const toast = document.createElement('div');
            toast.className = 'alert alert-success mt-2 text-center';
            toast.innerText = 'Link copiado!';
            input.parentNode.appendChild(toast);
            setTimeout(() => toast.remove(), 2000);
        }

    }

    var entrarLobbyPageConvidado = document.getElementById('entrarLobbyPageConvidado');
    if (entrarLobbyPageConvidado) {

        console.log("🟢 Página entrarLobbyPage detectada");

        const codigoInput = document.getElementById('codigoSala');
        const nomeInput = document.getElementById('nomeJogador');
        const btnEntrar = document.getElementById('btnEntrarLobby');
        const codigo = codigoInput.value.trim();
        const cookieKey = `JogadorLobby_${codigo}`;

        // Se já tiver o cookie, redireciona direto
        if (getCookie(cookieKey)) {
            console.log("🔁 Jogador já entrou nessa sala. Redirecionando...");
            window.location.href = `/Sorteio?codigo=${codigo}`;
            return;
        }

        btnEntrar.addEventListener('click', async function () {
            console.log("🔘 Botão clicado para entrar no lobby");

            const nome = nomeInput.value.trim();

            if (!nome) {
                alert("Por favor, informe seu nome.");
                console.warn("⚠️ Nome está vazio, abortando.");
                return;
            }

            const payload = {
                Codigo: codigo,
                Nome: nome
            };

            console.log("📦 Payload sendo enviado:", payload);

            try {
                const response = await fetch('/Lobby/Entrar', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    const jogadorId = await response.text();
                    setCookie(cookieKey, jogadorId, 2);

                    console.log("🟢 Entrada confirmada. ID salvo no cookie:", jogadorId);
                    window.location.href = `/Lobby/${codigo}`;
                } else {
                    console.error("❌ Erro ao entrar na sala. Status:", response.status);
                    alert("Erro ao entrar na sala. Tente novamente.");
                }

            } catch (err) {
                console.error("🔥 Erro inesperado ao enviar requisição:", err);
                alert("Erro inesperado. Tente novamente mais tarde.");
            }
        });

        function setCookie(name, value, days) {
            const expires = new Date(Date.now() + days * 86400000).toUTCString();
            document.cookie = `${name}=${value}; expires=${expires}; path=/`;
        }

        function getCookie(name) {
            const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
            return match ? match[2] : null;
        }
    }

    var lobbyPublicoPage = document.getElementById('lobbyPublicoPage');
    if (lobbyPublicoPage) {
        console.log("✅ Página do lobby público carregada");

        const codigoLobby = document.getElementById('codigoSalaLobby').dataset.codigo;
        const listaJogadores = document.getElementById("listaJogadores");

        async function atualizarLista() {
            const response = await fetch(`/Lobby/Jogadores?codigo=${codigoLobby}`);
            if (!response.ok) return;

            const jogadores = await response.json();
            listaJogadores.innerHTML = "";

            jogadores.forEach((j, index) => {
                const li = document.createElement("li");
                li.className = "list-group-item";
                li.innerHTML = `<strong>${index + 1}.</strong> ${j}`;
                listaJogadores.appendChild(li);
            });
        }

        setInterval(atualizarLista, 5000);
        atualizarLista(); // Atualiza logo ao entrar

        const btnSair = document.getElementById("btnSairLobby");
        if (btnSair) {
            btnSair.addEventListener("click", async () => {
                let jogadorId = getCookie(`JogadorLobby_${codigoLobby}`);
                if (!jogadorId) return;

                jogadorId = jogadorId.replace(/"/g, '');

                const confirmed = confirm("Tem certeza que deseja sair da sala?");
                if (!confirmed) return;

                const response = await fetch(`/Lobby/Sair?codigo=${codigoLobby}&jogadorId=${jogadorId}`, {
                    method: "DELETE"
                });

                if (response.ok) {
                    // Remove o cookie
                    document.cookie = `JogadorLobby_${codigoLobby}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
                    window.location.reload(); // Recarrega para cair na tela de entrada
                } else {
                    alert("Erro ao sair da sala.");
                }
            });
        }
    }

    // Função utilitária para ler cookie
    function getCookie(name) {
        const cookies = document.cookie.split(';');
        for (let c of cookies) {
            const [k, v] = c.trim().split('=');
            if (k === name) return v;
        }
        return null;
    }


});