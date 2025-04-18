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

        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const email = document.getElementById('email').value;
            const senha = senhaInput.value;

            // Ativa estado de carregamento
            btn.disabled = true;
            btnIcon.classList.add('d-none');
            btnText.textContent = "Entrando...";
            btnSpinner.classList.remove('d-none');

            // CHAMADA PARA O MÉTODO MVC
            const formData = new FormData();
            formData.append('email', email);
            formData.append('senha', senha);

            fetch('/Login/Logar', {
                method: 'POST',
                body: formData
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

        const sucesso = getUrlParameter('sucesso');
        if (sucesso === 'senha') {
            mostrarToast('Senha redefinida com sucesso! Faça o login.', 'success');
            // limpa a query string para não repetir ao atualizar a página
            window.history.replaceState({}, document.title, window.location.pathname);
        }
    }


    var sorteioPage = document.getElementById('sorteioPage');
    if (sorteioPage) {
        const teams = [];
        let playerToTransfer = null;
        let editablePlayer = null;

        function salvarLocalizacaoCookie(lat, long) {
            document.cookie = `localizacao=${lat},${long}; path=/; max-age=86400`;
            mostrarToast(`Localização salva: ${lat}, ${long}`, 'success');
        }

        function solicitarLocalizacao() {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(
                    (pos) => {
                        const { latitude, longitude } = pos.coords;
                        salvarLocalizacaoCookie(latitude, longitude);
                    },
                    (err) => {
                        mostrarToast('Não foi possível obter a localização.', 'danger');
                        console.error(err);
                    }
                );
            } else {
                mostrarToast('Geolocalização não é suportada neste navegador.', 'warning');
            }
        }

        solicitarLocalizacao();

        function mostrarToast(message, type = 'info') {
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
                mostrarToast('Informe a quantidade de jogadores por time.', 'warning');
                return;
            }

            if (playerNames.length === 0) {
                mostrarToast('Adicione jogadores antes de gerar os times!', 'warning');
                return;
            }

            if (hasFixedGoalkeeper && goalkeeperNames.length === 0) {
                mostrarToast('Informe pelo menos um goleiro fixo antes de gerar os times!', 'warning');
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

            // Envia para o backend se estiver no modo lobby
            if (isLobbyMode()) {
                const codigo = new URLSearchParams(window.location.search).get('codigo');

                fetch('/Sorteio/Criar', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        CodigoLobby: codigo,
                        NomeSorteio: `Sorteio - ${new Date().toLocaleTimeString()}`,
                        Times: teams.map(team => ({
                            Nome: team.name,
                            CorHex: team.color,
                            Jogadores: team.players,
                            Goleiro: team.goalkeeper?.replace('🧤 ', '') || ''
                        }))
                    })
                })
                    .then(res => res.json())
                    .then(res => {
                        if (res.sucesso) {
                            mostrarToast('Sorteio salvo com sucesso!', 'success');
                        } else {
                            mostrarToast('Erro ao salvar o sorteio!', 'danger');
                        }
                    })
                    .catch(() => mostrarToast('Erro ao se comunicar com o servidor.', 'danger'));
            }

            renderTeams();
            document.getElementById("btnResortear").style.display = "block";
            mostrarToast('Times gerados automaticamente!', 'success');
            const btnConfronto = document.getElementById("btnConfronto");
            if (btnConfronto) {
                const codigo = new URLSearchParams(window.location.search).get("codigo");
                btnConfronto.href = `/Confronto?codigo=${codigo}`;
                btnConfronto.classList.remove("d-none");
            }
        }

        function renderTeams() {
            const container = document.getElementById('teamsContainer');
            console.log("🔄 Chamando renderTeams()");
            console.log("📦 Container encontrado:", container);
            console.log("📊 Lista de teams:", teams);

            if (!container) {
                console.error("❌ Container 'teamsContainer' não encontrado no DOM.");
                return;
            }

            if (isLobbyMode()) {
                pararAtualizacaoLobby = true;
            }

            container.innerHTML = '';

            teams.forEach((team, teamIndex) => {
                console.log(`➡️ Renderizando ${team.name}`, team);

                const isIncomplete = team.players.length < parseInt(document.getElementById('playersPerTeam').value);
                const cardClass = isIncomplete ? 'team-card incomplete' : 'team-card';

                const goalkeeperHTML = team.goalkeeper ? `
                    <h6 class="text-white">Gol:</h6>
                    <ul class="list-group mb-3">
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span>${team.goalkeeper}</span>
                        </li>
                    </ul>` : '';

                const linePlayersHTML = team.players.map((player, playerIndex) => {
                    console.log(`👤 Jogador: ${player} (Team ${teamIndex}, Index ${playerIndex})`);
                    return `
                        <li class="list-group-item d-flex justify-content-between align-items-center">
                            <span>${playerIndex + 1} - ${renderPlayerName(player, teamIndex, playerIndex)}</span>
                            <div class="btn-group">
                                <button class="btn btn-sm btn-info transfer-btn" data-team="${teamIndex}" data-player="${playerIndex}">Transferir</button>
                                <button class="btn btn-sm btn-warning edit-btn" data-team="${teamIndex}" data-player="${playerIndex}">Editar</button>
                            </div>
                        </li>`;
                }).join('');

                const cardHTML = `
                <div class="col-md-4" data-aos="fade-up">
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

                container.innerHTML += cardHTML;
                console.log(`🧩 HTML gerado para ${team.name}:
                `, cardHTML);
            });

            // 🔗 Reanexa eventos
            console.log("🔗 Reanexando eventos de edição e transferência...");
            document.querySelectorAll('.edit-btn').forEach(btn =>
                btn.addEventListener('click', e => enableEditing(+btn.dataset.team, +btn.dataset.player))
            );
            document.querySelectorAll('.transfer-btn').forEach(btn =>
                btn.addEventListener('click', e => openTransferModal(+btn.dataset.team, +btn.dataset.player))
            );

            // ✨ Força animação AOS (em caso de não ser aplicada automaticamente)
            if (window.AOS && typeof AOS.refresh === 'function') {
                AOS.refresh();
                console.log('✨ AOS.refresh() chamado');
            }

            // ✅ Força a visibilidade no caso do AOS não aplicar a animação
            document.querySelectorAll('[data-aos]').forEach(el => el.classList.add('aos-animate'));

            console.log("✅ Finalizado renderTeams()");
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
                mostrarToast('Nome do jogador atualizado com sucesso!', 'success');
            } else {
                mostrarToast('Nome inválido! A edição foi cancelada.', 'warning');
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
            mostrarToast('Jogadores trocados com sucesso!', 'success');
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
        let jogadoresAntigos = [];

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

                    console.log("📥 Jogadores do servidor:", jogadoresServidor);
                    console.log("🧠 Jogadores antigos:", jogadoresAntigos);

                    // Extrai apenas os nomes vindos do servidor
                    const nomesServidor = jogadoresServidor.map(j => j.nome);

                    // Lista atual do textarea
                    let jogadoresAtuais = textarea.value
                        .split('\n')
                        .map(j => j.trim())
                        .filter(j => j);

                    const novos = nomesServidor.filter(nome => !jogadoresAtuais.includes(nome));
                    const sairam = jogadoresAntigos.filter(nome => !nomesServidor.includes(nome));

                    // ✅ Toast + adiciona os novos
                    novos.forEach(nome => {
                        if (nome) {
                            mostrarToast(`🎉 ${nome} entrou no lobby!`, 'success');
                            jogadoresAtuais.push(nome);
                        }
                    });

                    // ✅ Toast + remove os que saíram
                    sairam.forEach(nome => {
                        if (nome) {
                            mostrarToast(`👋 ${nome} saiu do lobby.`, 'warning');
                            jogadoresAtuais = jogadoresAtuais.filter(j => j !== nome);
                        }
                    });

                    jogadoresAntigos = [...nomesServidor];

                    // Atualiza visual
                    textarea.value = jogadoresAtuais.join('\n');
                    count.textContent = `${jogadoresAtuais.length} jogador(es) adicionado(s)`;
                    list.innerHTML = '';
                    jogadoresAtuais.forEach((j, i) => {
                        const li = document.createElement('li');
                        li.className = 'list-group-item show';
                        li.innerHTML = `<span>${i + 1} - ${j}</span>`;
                        list.appendChild(li);
                    });

                } catch (err) {
                    console.error('❌ Erro ao buscar jogadores do lobby:', err);
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


        // Função de copiar link
        var btnCopiarLink = document.getElementById('btnCopiarLink');
        if (btnCopiarLink) {
            btnCopiarLink.addEventListener('click', function () {
                var inputLink = document.getElementById('linkCompartilhavel');
                if (inputLink) {
                    inputLink.select();
                    inputLink.setSelectionRange(0, 99999); // para mobile
                    document.execCommand("copy");

                    // Exibir toast
                    mostrarToast('Link copiado com sucesso!');

                    // Feedback visual no botão (opcional)
                    btnCopiarLink.innerHTML = '<i class="fas fa-check me-1"></i> Copiado!';
                    setTimeout(() => {
                        btnCopiarLink.innerHTML = '<i class="fas fa-copy me-1"></i> Copiar';
                    }, 2000);
                }
            });
        }

        const btnAbrirChatAdmin = document.getElementById("btnAbrirChatAdmin");
        if (btnAbrirChatAdmin) {
            btnAbrirChatAdmin.style.display = "inline-flex";
        }
        const chatMensagens = document.createElement("div");
        const chatInputContainer = document.createElement("div");
        const listaOnline = document.createElement("ul");
        const codigoLobby = new URLSearchParams(window.location.search).get("codigo");

        const dadosJogador = document.getElementById("dadosJogador");
        const jogadorId = dadosJogador?.dataset.id || "admin";
        const jogadorNome = dadosJogador?.dataset.nome || "Admin";

        let chatAtivo = false;
        let jogadoresAtividade = {};

        if (btnAbrirChatAdmin) {
            chatMensagens.id = "chatMensagensAdmin";
            chatMensagens.className = "bg-light text-dark rounded p-2 mb-2 small";
            chatMensagens.style.height = "250px";
            chatMensagens.style.overflowY = "auto";

            listaOnline.id = "listaJogadoresOnline";
            listaOnline.className = "list-group mb-3 small";
            listaOnline.style.maxHeight = "150px";
            listaOnline.style.overflowY = "auto";

            chatInputContainer.className = "d-flex";
            chatInputContainer.innerHTML = `
        <input type="text" id="inputMensagemAdmin" class="form-control me-2" placeholder="Digite..." />
        <button id="btnEnviarMensagemAdmin" class="btn btn-success">
            <i class="fas fa-paper-plane"></i>
        </button>
    `;

            const chatBox = document.createElement("div");
            chatBox.id = "chatAdminContainer";
            chatBox.className = "position-fixed end-0 bottom-0 bg-dark text-white p-3 shadow-lg rounded-start animate__animated";
            chatBox.style.width = "320px";
            chatBox.style.maxHeight = "90vh";
            chatBox.style.display = "none";
            chatBox.style.zIndex = 10000;
            chatBox.style.borderLeft = "4px solid #3d7fff";

            chatBox.innerHTML = `
        <div class="d-flex justify-content-between align-items-center mb-2">
            <strong><i class="fas fa-comment-dots me-2"></i>Chat da Sala</strong>
            <button id="btnFecharChatAdmin" class="btn btn-sm btn-outline-light">
                <i class="fas fa-times"></i>
            </button>
        </div>
        <div class="mb-2">
            <strong class="text-white small"><i class="fas fa-circle text-success me-1"></i>Jogadores Online</strong>
        </div>
    `;
            chatBox.appendChild(listaOnline);
            chatBox.appendChild(chatMensagens);
            chatBox.appendChild(chatInputContainer);
            document.body.appendChild(chatBox);

            const btnEnviar = chatInputContainer.querySelector("#btnEnviarMensagemAdmin");
            const inputMensagem = chatInputContainer.querySelector("#inputMensagemAdmin");

            function adicionarMensagem(nome, mensagem, hora) {
                if (chatMensagens.querySelector("p.text-muted")) chatMensagens.innerHTML = '';
                const div = document.createElement("div");
                div.className = "mb-2";
                div.innerHTML = `
            <div class="bg-light p-2 rounded shadow-sm">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <strong><i class="fas fa-user me-1"></i>${nome}</strong>
                        <p class="mb-0">${mensagem}</p>
                    </div>
                    <small class="text-muted ms-2">${hora}</small>
                </div>
            </div>
        `;
                chatMensagens.appendChild(div);
                chatMensagens.scrollTop = chatMensagens.scrollHeight;
            }

            function atualizarListaOnline(nomes) {
                listaOnline.innerHTML = "";
                nomes.forEach(nome => {
                    const li = document.createElement("li");
                    const ultimaAtividade = jogadoresAtividade[nome];
                    const inativoMin = ultimaAtividade ? getMinutosInativo(ultimaAtividade) : 0;

                    const status = inativoMin < 3
                        ? `<span class="badge bg-success">🟢 Ativo</span>`
                        : `<span class="badge bg-secondary">${inativoMin} min</span>`;

                    li.className = "list-group-item d-flex justify-content-between align-items-center bg-dark text-white border-0 px-2 py-1";
                    li.innerHTML = `<span><i class="fas fa-user me-2"></i>${nome}</span>${status}`;
                    listaOnline.appendChild(li);
                });
            }

            function getMinutosInativo(dataIso) {
                const ultimo = new Date(dataIso);
                const agora = new Date();
                return Math.floor((agora - ultimo) / 60000);
            }

            function formatarHora(dataIso) {
                const data = new Date(dataIso);
                return data.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" });
            }

            const connection = new signalR.HubConnectionBuilder()
                .withUrl(`/hubs/lobbychat?codigoSala=${codigoLobby}&jogadorId=${jogadorId}&nome=${jogadorNome}`)
                .withAutomaticReconnect()
                .configureLogging(signalR.LogLevel.Information)
                .build();

            connection.on("ReceberMensagem", (nome, mensagem, hora) => {
                adicionarMensagem(nome, mensagem, hora);
            });

            connection.on("AtualizarUsuariosOnline", async (identificadoresOnline) => {
                try {
                    const res = await fetch(`/Lobby/Jogadores?codigo=${codigoLobby}`);
                    if (!res.ok) return;

                    const jogadores = await res.json();

                    // Mapeamento identificador → nome e últimaAtividade
                    const mapaNomes = {};
                    jogadoresAtividade = {};

                    jogadores.forEach(j => {
                        const identificador = j.identificador || j.nome;
                        mapaNomes[identificador] = j.nome;
                        jogadoresAtividade[j.nome] = j.ultimaAtividade;
                    });

                    // Traduz identificadoresOnline para nomes
                    const nomes = identificadoresOnline
                        .map(id => mapaNomes[id])
                        .filter(nome => !!nome); // remove nulos

                    atualizarListaOnline(nomes);
                } catch (e) {
                    console.warn("Erro ao buscar jogadores para mapear nomes:", e);
                }
            });



            connection.start().then(async () => {
                try {
                    const resMensagens = await fetch(`/Lobby/Mensagens?codigo=${codigoLobby}`);
                    const mensagens = await resMensagens.json();
                    mensagens.forEach(m => adicionarMensagem(m.nomeUsuario, m.conteudo, formatarHora(m.dataEnvio)));

                    const resJogadores = await fetch(`/Lobby/Jogadores?codigo=${codigoLobby}`);
                    const jogadores = await resJogadores.json();
                    jogadoresAtividade = {};
                    jogadores.forEach(j => {
                        jogadoresAtividade[j.nome] = j.ultimaAtividade;
                    });

                    const resOnline = await fetch(`/Lobby/UsuariosOnline?codigo=${codigoLobby}`);
                    const online = await resOnline.json();
                    atualizarListaOnline(online);
                } catch {
                    adicionarMensagem("Sistema", "Erro ao carregar dados", "⚠️");
                }
            });

            setInterval(async () => {
                try {
                    // Atualiza jogadores e o mapa de nomes
                    const res = await fetch(`/Lobby/Jogadores?codigo=${codigoLobby}`);
                    const jogadores = await res.json();

                    jogadoresAtividade = {};
                    mapaNomes = {};

                    jogadores.forEach(j => {
                        const identificador = j.identificador || j.nome;
                        mapaNomes[identificador] = j.nome;
                        jogadoresAtividade[j.nome] = j.ultimaAtividade;
                    });

                    // Agora pega quem está online e converte ID → Nome
                    const resOnline = await fetch(`/Lobby/UsuariosOnline?codigo=${codigoLobby}`);
                    const online = await resOnline.json();

                    const nomes = online.map(id => mapaNomes[id]).filter(n => !!n);

                    atualizarListaOnline(nomes);
                } catch (e) {
                    console.error("❌ Erro ao atualizar jogadores online:", e);
                }
            }, 15000);

            btnEnviar.addEventListener("click", async () => {
                const mensagem = inputMensagem.value.trim();
                if (!mensagem) return;
                await connection.invoke("EnviarMensagem", codigoLobby, jogadorId, mensagem);
                inputMensagem.value = "";
            });

            inputMensagem.addEventListener("keydown", async (e) => {
                if (e.key === "Enter") {
                    e.preventDefault();
                    btnEnviar.click();
                }
            });

            btnAbrirChatAdmin.addEventListener("click", () => {
                const chatBox = document.getElementById("chatAdminContainer");
                chatBox.style.display = "block";
                chatBox.classList.add("animate__slideInRight");
                btnAbrirChatAdmin.style.display = "none";
                chatAtivo = true;
            });

            document.addEventListener("click", function (e) {
                if (e.target.id === "btnFecharChatAdmin") {
                    const chatBox = document.getElementById("chatAdminContainer");
                    chatBox.classList.remove("animate__slideInRight");
                    chatBox.style.display = "none";
                    chatAtivo = false;

                    setTimeout(() => {
                        btnAbrirChatAdmin.style.display = "inline-block";
                    }, 300);
                }
            });
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
        const btnMostrarMais = document.getElementById('btnMostrarMais');
        const mostrarMaisContainer = document.getElementById('mostrarMaisContainer');

        let paginaAtual = 1;
        const quantidadePorPagina = 10;
        let carregando = false;

        carregarPagina(); // carrega a primeira

        btnMostrarMais.addEventListener('click', () => {
            if (!carregando) {
                carregarPagina(); // carrega próxima página automaticamente
            }
        });

        function carregarPagina() {
            carregando = true;
            mostrarPlaceholders();

            fetch(`/Partida/ObterHistoricoJson?pagina=${paginaAtual}&quantidade=${quantidadePorPagina}`)
                .then(response => {
                    if (!response.ok) throw new Error('Erro ao carregar o histórico de partidas.');
                    return response.json();
                })
                .then(partidas => {
                    removePlaceholders();

                    if (paginaAtual === 1) container.innerHTML = ''; // limpa só na 1ª

                    if (partidas.length === 0) {
                        mostrarMaisContainer.style.display = 'none';
                        if (paginaAtual === 1) {
                            container.innerHTML = `
                            <div class="col">
                                <div class="alert alert-info text-center w-100">
                                    Nenhuma partida encontrada.
                                </div>
                            </div>`;
                        } else {
                            container.innerHTML += `
                            <div class="col">
                                <div class="alert alert-info text-center w-100">
                                    🏁 Fim do histórico. Nenhuma partida a mais encontrada.
                                </div>
                            </div>`;
                        }
                        return;
                    }

                    const exibirIncremental = partidas.length > 2;

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
                            </div>`;
                            container.appendChild(col);
                        }, delay);
                    });

                    mostrarMaisContainer.style.display = 'block';
                    paginaAtual++; // <-- incrementa após carregamento bem-sucedido
                    carregando = false;
                })
                .catch(error => {
                    removePlaceholders();
                    container.innerHTML += `
                    <div class="col">
                        <div class="alert alert-danger text-center w-100">${error.message}</div>
                    </div>`;
                    carregando = false;
                });
        }

        function mostrarPlaceholders() {
            for (let i = 0; i < quantidadePorPagina; i++) {
                const placeholder = document.createElement('div');
                placeholder.className = 'col placeholder-loading';
                placeholder.innerHTML = `
                <div class="card border-0 shadow-lg h-100 rounded-4 placeholder-glow">
                    <div class="card-header bg-dark text-white d-flex justify-content-between align-items-center rounded-top-4">
                        <h5 class="placeholder col-6 mb-0">&nbsp;</h5>
                        <span class="placeholder col-3 badge bg-secondary">&nbsp;</span>
                    </div>
                    <div class="card-body">
                        <p class="mb-1"><strong>Local:</strong></p>
                        <p class="placeholder col-8"></p>
                        <div class="rounded-3 overflow-hidden mb-3 bg-secondary placeholder" style="height: 200px;"></div>
                        <div class="placeholder btn btn-outline-primary disabled w-100">&nbsp;</div>
                    </div>
                </div>`;
                container.appendChild(placeholder);
            }
        }

        function removePlaceholders() {
            document.querySelectorAll('.placeholder-loading').forEach(el => el.remove());
        }

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
        formCriarSala.addEventListener('submit', async (e) => {
            e.preventDefault(); // evita o envio até obter localização

            if (!navigator.geolocation) {
                mostrarToast("Geolocalização não suportada pelo navegador.", false);
                formCriarSala.submit(); // ainda assim envia a sala sem localização
                return;
            }

            navigator.geolocation.getCurrentPosition((pos) => {
                const latitude = pos.coords.latitude.toFixed(7);
                const longitude = pos.coords.longitude.toFixed(7);

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

                formCriarSala.submit();
            }, (err) => {
                console.error("Erro ao obter localização:", err);
                mostrarToast("Não foi possível obter sua localização atual.", false);
                formCriarSala.submit(); // continua mesmo sem localização
            });
        });
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
        const emailInput = document.getElementById('emailJogador');
        const btnEntrar = document.getElementById('btnEntrarLobby');
        const codigo = codigoInput.value.trim();
        const cookieKey = `JogadorLobby_${codigo}`;

        if (getCookie(cookieKey)) {
            console.log("🔁 Jogador já entrou nessa sala. Redirecionando...");
            window.location.href = `/Sorteio?codigo=${codigo}`;
            return;
        }

        btnEntrar.addEventListener('click', async function () {
            const nome = nomeInput.value.trim();
            const email = emailInput.value.trim();

            if (!nome) {
                mostrarToast("Por favor, informe seu nome.", false);
                return;
            }

            if (!email) {
                mostrarToast("Por favor, informe seu e-mail.", false);
                return;
            }

            if (!validarEmail(email)) {
                mostrarToast("E-mail inválido. Verifique e tente novamente.", false);
                return;
            }

            const payload = {
                Codigo: codigo,
                Nome: nome,
                Email: email,
                UsuarioId: null
            };

            try {
                const response = await fetch('/Lobby/Entrar', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    const jogador = await response.json();

                    setCookie(`JogadorLobbyId_${codigo}`, jogador.id, 2);
                    setCookie(`JogadorLobbyNome_${codigo}`, jogador.nome, 2);

                    mostrarToast("🎉 Você entrou na sala!", true);
                    setTimeout(() => {
                        window.location.href = `/Lobby/${codigo}`;
                    }, 1000);
                } else {
                    mostrarToast("Erro ao entrar na sala. Tente novamente.", false);
                }

            } catch (err) {
                mostrarToast("Erro inesperado. Verifique sua conexão.", false);
            }
        });

        function validarEmail(email) {
            const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return regex.test(email);
        }

        function setCookie(name, value, days) {
            const expires = new Date(Date.now() + days * 86400000).toUTCString();
            document.cookie = `${name}=${value}; expires=${expires}; path=/`;
        }

        function getCookie(name) {
            const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
            return match ? match[2] : null;
        }
    }


    window.reagir = function (elemento, emoji) {
        const contador = elemento.querySelector("sup");
        if (contador) {
            let atual = parseInt(contador.innerText);
            contador.innerText = atual + 1;
        } else {
            elemento.innerHTML += ` <sup class="text-muted">1</sup>`;
        }
    };

    function formatarHora(dataIso) {
        const data = new Date(dataIso);
        return data.toLocaleTimeString("pt-BR", { hour: "2-digit", minute: "2-digit" });
    }

    const lobbyPublicoPage = document.getElementById('lobbyPublicoPage');
    if (lobbyPublicoPage) {
        let mapaIdentificadores = {};
        const codigoLobby = document.getElementById('codigoSalaLobby').dataset.codigo;
        const listaJogadores = document.getElementById("listaJogadores");
        const dadosJogador = document.getElementById("dadosJogador");
        const jogadorAtualId = dadosJogador.dataset.id;
        const jogadorAtualNome = dadosJogador.dataset.nome;
        const chatMensagens = document.getElementById("chatMensagens");
        const inputMensagem = document.getElementById("inputMensagem");
        const btnEnviar = document.getElementById("btnEnviarMensagem");
        const typingStatus = document.getElementById("statusDigitando");
        const toggleSwitch = document.getElementById("toggleNotificacoes");
        const audioNotification = new Audio('/sounds/notification_message.mp3');

        let digitandoTimeouts = {};
        let mensagensNaoLidas = 0;
        const tituloOriginal = document.title;
        const digitandoUsuarios = new Set();

        function salvarPreferenciaNotificacao(ativa) {
            localStorage.setItem("notificacaoPushAtiva", ativa ? "1" : "0");
        }

        function estaNotificacaoAtiva() {
            return localStorage.getItem("notificacaoPushAtiva") === "1" && Notification.permission === "granted";
        }

        async function inicializarToggleNotificacoes() {
            if (!("Notification" in window) || !toggleSwitch) return;
            const permissao = Notification.permission;
            const localPref = localStorage.getItem("notificacaoPushAtiva");
            toggleSwitch.checked = (permissao === "granted" && localPref === "1");

            toggleSwitch.addEventListener("change", async () => {
                if (toggleSwitch.checked) {
                    if (Notification.permission === "granted") {
                        salvarPreferenciaNotificacao(true);
                    } else if (Notification.permission !== "denied") {
                        const perm = await Notification.requestPermission();
                        if (perm === "granted") {
                            salvarPreferenciaNotificacao(true);
                        } else {
                            toggleSwitch.checked = false;
                            salvarPreferenciaNotificacao(false);
                            alert("Você precisa permitir notificações no navegador.");
                        }
                    } else {
                        toggleSwitch.checked = false;
                        salvarPreferenciaNotificacao(false);
                        alert("As notificações estão bloqueadas nas configurações do navegador.");
                    }
                } else {
                    salvarPreferenciaNotificacao(false);
                }
            });
        }

        inicializarToggleNotificacoes();

        document.addEventListener("visibilitychange", () => {
            if (!document.hidden) {
                mensagensNaoLidas = 0;
                document.title = tituloOriginal;
            }
        });

        async function carregarHistoricoMensagens() {
            try {
                const res = await fetch(`/Lobby/Mensagens?codigo=${codigoLobby}`);
                if (!res.ok) return;
                const mensagens = await res.json();
                if (mensagens.length > 0) {
                    const mensagemVazia = chatMensagens.querySelector(".text-muted.text-center");
                    if (mensagemVazia) mensagemVazia.remove();
                }

                mensagens.forEach(m => {
                    const hora = formatarHora(m.dataEnvio);
                    adicionarMensagemNoChat(m.nomeUsuario, m.conteudo, hora);
                });
            } catch (err) {
                console.error("❌ Erro ao buscar histórico:", err);
            }
        }

        function adicionarMensagemNoChat(nome, mensagem, horario) {
            const novaMsg = document.createElement("div");
            novaMsg.className = "mb-2";
            novaMsg.innerHTML = `
        <div class="bg-light p-2 rounded shadow-sm">
            <div class="d-flex justify-content-between align-items-start">
                <div>
                    <strong><i class="fas fa-user me-1"></i>${nome}</strong>
                    <p class="mb-0">${mensagem}</p>
                </div>
                <small class="text-muted ms-2">${horario}</small>
            </div>
            <div class="reacoes mt-2">
                <span role="button" onclick="reagir(this, '👍')">👍</span>
                <span role="button" onclick="reagir(this, '🔥')">🔥</span>
                <span role="button" onclick="reagir(this, '😂')">😂</span>
                <span role="button" onclick="reagir(this, '❤️')">❤️</span>
            </div>
        </div>`;
            chatMensagens.appendChild(novaMsg);
            chatMensagens.scrollTop = chatMensagens.scrollHeight;

            const deveNotificar = estaNotificacaoAtiva() && nome !== jogadorAtualNome;
            if (document.hidden) {
                mensagensNaoLidas++;
                document.title = `(${mensagensNaoLidas}) Nova mensagem - ${tituloOriginal}`;
            }

            if (deveNotificar) {
                try {
                    new Notification(`Nova mensagem de ${nome}`, {
                        body: mensagem,
                        icon: "/favicon.ico"
                    });
                    audioNotification.play().catch(() => { });
                } catch (e) {
                    console.warn("❌ Notificação falhou:", e);
                }
            }
        }

        async function atualizarLista() {
            try {
                const res = await fetch(`/Lobby/Jogadores?codigo=${codigoLobby}`);
                if (!res.ok) return;
                const jogadores = await res.json();
                listaJogadores.innerHTML = "";
                mapaIdentificadores = {};

                jogadores.forEach((j, i) => {
                    const idInterno = `status-${(j.identificador || j.nome).replace(/\s/g, "-")}`;
                    const hora = new Date(j.ultimaAtividade).toLocaleTimeString("pt-BR", {
                        hour: "2-digit",
                        minute: "2-digit"
                    });

                    const li = document.createElement("li");
                    li.className = "list-group-item d-flex justify-content-between align-items-center";
                    li.innerHTML = `
        <span>
            <i class="fas fa-futbol me-2 text-secondary"></i>
            <strong>${i + 1}.</strong> ${j.nome}
            <small class="d-block text-muted">Visto por último às ${hora}</small>
        </span>
        <span class="badge rounded-pill" id="${idInterno}">
            <i class="fas fa-circle" style="color: #dc3545;"></i>
        </span>`;

                    listaJogadores.appendChild(li);
                    mapaIdentificadores[j.identificador] = idInterno;
                });

                atualizarStatusUsuariosOnline();
            } catch (err) {
                console.error("❌ Erro ao atualizar jogadores:", err);
            }
        }

        async function atualizarStatusUsuariosOnline() {
            try {
                const res = await fetch(`/Lobby/UsuariosOnline?codigo=${codigoLobby}`);
                if (!res.ok) return;

                const usuariosOnline = await res.json();
                usuariosOnline.forEach(identificador => {
                    const id = mapaIdentificadores[identificador];
                    if (!id) return;

                    const badge = document.getElementById(id);
                    if (badge) {
                        badge.innerHTML = `<i class="fas fa-circle" style="color: #28a745;"></i>`;
                    }
                });
            } catch (err) {
                console.error("❌ Erro ao verificar online:", err);
            }
        }

        async function verificarSorteio() {
            try {
                const res = await fetch(`/Lobby/VerificarSorteio?codigo=${codigoLobby}`);
                if (!res.ok) return;
                const data = await res.json();
                const sorteio = data.sorteio;
                if (!sorteio || !sorteio.times?.length) return;

                const container = document.getElementById('timesSorteadosContainer');
                container.innerHTML = "<h5 class='text-center text-white mt-4'>🏆 Times Sorteados</h5>";

                const timesOrdenados = [...sorteio.times].sort((a, b) => {
                    return parseInt(a.nome.replace(/\D/g, '')) - parseInt(b.nome.replace(/\D/g, ''));
                });

                timesOrdenados.forEach(time => {
                    const contemJogadorAtual = time.jogadores.some(j => j.id === jogadorAtualId);
                    const card = document.createElement("div");
                    card.className = `card shadow-sm mb-3 time-card ${contemJogadorAtual ? "destaque-time-jogador" : ""}`;
                    const jogadoresHTML = time.jogadores.map(j => {
                        const isJogadorAtual = j.id === jogadorAtualId;
                        return `<li class="list-group-item ${isJogadorAtual ? " fw-bold text-success" : ""}">${j.nome}</li>`;
                    }).join("");
                    card.innerHTML = `
            < div class="card-header bg-dark text-white" > ${time.nome}</div >
                <ul class="list-group list-group-flush">${jogadoresHTML}</ul>`;
                    container.appendChild(card);
                });
            } catch (err) {
                console.error("❌ Erro ao verificar sorteio:", err);
            }
        }

        const btnSair = document.getElementById("btnSairLobby");
        if (btnSair) {
            btnSair.addEventListener("click", async () => {
                const url = `/Lobby/Sair?codigo=${encodeURIComponent(codigoLobby)}&jogadorId=${encodeURIComponent(jogadorAtualId)}`;
                const res = await fetch(url, { method: "DELETE" });
                if (res.ok) {
                    document.cookie = `JogadorLobbyId_${codigoLobby}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
                    document.cookie = `JogadorLobbyNome_${codigoLobby}=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;`;
                    window.location.reload();
                }
            });
        }
        const connection = new signalR.HubConnectionBuilder()
            .withUrl(`/hubs/lobbychat?codigoSala=${codigoLobby}&jogadorId=${jogadorAtualId}&nome=${jogadorAtualNome}`)
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Information)
            .build();

        btnEnviar.addEventListener("click", async () => {
            const mensagem = inputMensagem.value.trim();
            if (!mensagem) return;
            await connection.invoke("EnviarMensagem", codigoLobby, jogadorAtualId, mensagem);
            inputMensagem.value = "";
        });

        inputMensagem.addEventListener("input", () => {
            connection.invoke("UsuarioDigitando", codigoLobby, jogadorAtualId);
        });

        connection.on("MostrarDigitando", (nome) => {
            digitandoUsuarios.add(nome);
            atualizarStatusDigitando();
            clearTimeout(digitandoTimeouts[nome]);
            digitandoTimeouts[nome] = setTimeout(() => {
                digitandoUsuarios.delete(nome);
                atualizarStatusDigitando();
            }, 3000);
        });

        function atualizarStatusDigitando() {
            const nomes = Array.from(digitandoUsuarios).filter(n => n !== jogadorAtualNome);
            typingStatus.innerText = nomes.length > 0
                ? `${nomes.join(", ")} ${nomes.length > 1 ? "estão" : "está"} digitando...`
                : "";
        }

        connection.on("ReceberMensagem", (nome, mensagem, hora) => {
            adicionarMensagemNoChat(nome, mensagem, hora);
        });

        connection.on("AtualizarUsuariosOnline", atualizarStatusUsuariosOnline);

        connection.start().then(() => {
            console.log("🟢 Conectado ao chat via SignalR");
            setTimeout(() => {
                atualizarLista();
            }, 500);
        });

        setInterval(atualizarLista, 10000);
        setInterval(verificarSorteio, 5000);
        carregarHistoricoMensagens();
        verificarSorteio();
    }



    // Função utilitária
    function getCookie(name) {
        const match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
        return match ? decodeURIComponent(match[2]) : null;
    }

    var minhasSalasPage = document.getElementById('minhasSalasPage');
    if (minhasSalasPage) {

        let paginaAtual = 1;
        const tamanhoPagina = 10;
        let carregando = false;

        async function carregarSalas() {
            if (carregando) return;
            carregando = true;

            const btn = document.getElementById('btnCarregarMais');
            if (btn) {
                btn.disabled = true;
                btn.innerHTML = '⏳ Carregando...';
            }

            const container = document.getElementById('salasContainer');
            if (paginaAtual === 1) {
                container.innerHTML = `
                <div class="spinner-container">
                    <span class="loader"></span>
                </div>`;
            }

            try {
                const response = await fetch(`/Sorteio/ObterSalas?pagina=${paginaAtual}&tamanhoPagina=${tamanhoPagina}`);
                if (!response.ok) throw new Error('Erro na requisição');
                const data = await response.json();

                if (paginaAtual === 1) container.innerHTML = '';

                if (data.length === 0) {
                    if (paginaAtual === 1) {
                        container.innerHTML = '<div class="alert alert-info text-center">Nenhuma sala encontrada.</div>';
                    } else {
                        mostrarToast('Nenhuma sala adicional encontrada.', 'info');
                    }
                    if (btn) btn.classList.add('d-none');
                    return;
                }

                for (const sala of data) {
                    const status = sala.finalizada ? "Finalizada" : "Aberta";
                    const badgeClass = sala.finalizada ? "bg-success" : "bg-warning text-dark";
                    const mapaUrl = `https://www.openstreetmap.org/export/embed.html?bbox=${sala.longitude},${sala.latitude},${sala.longitude},${sala.latitude}&layer=mapnik&marker=${sala.latitude},${sala.longitude}`;
                    const link = `${location.origin}/Sorteio?codigo=${sala.codigo}`;

                    const card = document.createElement('div');
                    card.className = 'card mb-4 shadow sala-item';
                    card.setAttribute('data-status', sala.finalizada ? 'finalizadas' : 'abertas');
                    card.setAttribute('data-codigo', sala.codigo);

                    // Requisição para endereço via Nominatim
                    let endereco = `Coordenadas (${sala.latitude.toFixed(4)}, ${sala.longitude.toFixed(4)})`;
                    try {
                        const resEndereco = await fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${sala.latitude}&lon=${sala.longitude}&zoom=18&addressdetails=1`);
                        const jsonEndereco = await resEndereco.json();
                        if (jsonEndereco && jsonEndereco.display_name) {
                            endereco = jsonEndereco.display_name;
                        }
                    } catch (e) {
                        console.warn('Erro ao obter endereço:', e);
                    }

                    card.innerHTML = `
                <div class="card-header bg-dark text-white d-flex justify-content-between align-items-center">
                    <strong>${new Date(sala.dataHora).toLocaleString()}</strong>
                    <span class="badge ${badgeClass}">${status}</span>
                </div>
                <div class="card-body">
                    <p><strong>Local:</strong> ${endereco}</p>
                    <p><strong>Código:</strong> <span class="text-primary">${sala.codigo}</span></p>

                    <div style="height: 300px; overflow: hidden; border-radius: 12px;" class="mb-3">
                        <iframe 
                            src="${mapaUrl}"
                            style="width: 100%; height: 100%; border: 0;"
                            allowfullscreen
                            loading="lazy"
                            referrerpolicy="no-referrer-when-downgrade">
                        </iframe>
                    </div>

                    <div class="input-group mb-3">
                        <input type="text" class="form-control" value="${link}" readonly id="link-${sala.partidaId}">
                        <button class="btn btn-outline-secondary" onclick="copiarLink('${sala.partidaId}')">📋 Copiar</button>
                    </div>

                    ${sala.finalizada
                            ? `<a href="/Partida/Detalhes/${sala.partidaId}" class="btn btn-outline-info w-100">
                               <i class="fas fa-info-circle me-2"></i> Ver Detalhes
                           </a>`
                            : `<a href="/Sorteio?codigo=${sala.codigo}" class="btn btn-primary w-100">
                               <i class="fas fa-eye me-2"></i> Ver Sorteio
                           </a>`
                        }
                </div>`;

                    container.appendChild(card);
                }

                paginaAtual++;
                carregando = false;

                if (data.length < tamanhoPagina) {
                    if (btn) btn.classList.add('d-none');
                    if (paginaAtual > 1) mostrarToast('Nenhuma sala adicional encontrada.', 'info');
                } else {
                    if (btn) {
                        btn.classList.remove('d-none');
                        btn.disabled = false;
                        btn.innerHTML = '<i class="fas fa-redo-alt me-2"></i> Carregar Mais';
                    }
                }

                aplicarFiltros();

            } catch (err) {
                console.error('Erro ao carregar salas:', err);
                mostrarToast('Erro ao carregar salas. Tente novamente.', 'danger');
                if (btn) btn.classList.remove('d-none');
            }
        }

        function copiarLink(id) {
            const input = document.getElementById('link-' + id);
            input.select();
            document.execCommand("copy");

            const toast = document.createElement('div');
            toast.className = 'alert alert-success mt-2 text-center';
            toast.innerText = 'Link copiado!';
            input.parentNode.appendChild(toast);
            setTimeout(() => toast.remove(), 2000);
        }

        function aplicarFiltros() {
            const statusSelecionado = document.getElementById('filtroStatus').value;
            const busca = document.getElementById('filtroBusca').value.toLowerCase();

            document.querySelectorAll('.sala-item').forEach(item => {
                const status = item.dataset.status;
                const codigo = item.dataset.codigo.toLowerCase();

                const statusOk = (statusSelecionado === 'todos' || statusSelecionado === status);
                const buscaOk = (codigo.includes(busca));

                item.style.display = (statusOk && buscaOk) ? 'block' : 'none';
            });
        }

        document.getElementById('filtroStatus').addEventListener('change', aplicarFiltros);
        document.getElementById('filtroBusca').addEventListener('input', aplicarFiltros);
        document.getElementById('btnCarregarMais').addEventListener('click', carregarSalas);

        carregarSalas();
    }



    var salasParticipadasPage = document.getElementById('salasParticipadasPage');
    if (salasParticipadasPage) {

        // AOS
        if (typeof AOS !== 'undefined') {
            AOS.init({
                duration: 800,
                easing: 'ease-in-out',
                once: true
            });
        }

        // Som de entrada
        const audio = new Audio('/sounds/notification_message.mp3');

        // Botão "Sorteio Rápido"
        const btnSorteio = salasParticipadasPage.querySelector('a[href="/Sorteio"]');
        if (btnSorteio) {
            btnSorteio.addEventListener('click', () => {
                console.log('➡️ Indo para o sorteio rápido...');
            });
        }

        // Hover nos cards
        const cards = salasParticipadasPage.querySelectorAll('.card');
        cards.forEach((card, index) => {
            card.addEventListener('mouseenter', () => {
                console.log(`🃏 Card ${index + 1} focado`);
            });
        });

        // Clique em "Entrar na Sala"
        const botoesEntrar = salasParticipadasPage.querySelectorAll('.btn-outline-light');
        botoesEntrar.forEach(botao => {
            botao.addEventListener('click', (e) => {
                e.preventDefault();

                // Toast + Som
                mostrarToast('Boa sorte no jogo! ⚽');
                if (audio) audio.play().catch(() => { });

                // Blur no conteúdo
                salasParticipadasPage.classList.add('blur-background');

                // Spinner no botão
                botao.classList.add('btn-loading');

                // Delay antes de redirecionar (ex: 1.5s)
                setTimeout(() => {
                    window.location.href = botao.getAttribute('href');
                }, 1500);
            });
        });
    }

    var cadastrarUsuarioPage = document.getElementById('cadastrarUsuarioPage');

    if (cadastrarUsuarioPage) {
        const form = document.getElementById("formCadastrar");
        const btn = document.getElementById("btnCadastrar");
        const btnText = document.getElementById("btnTextCadastro");
        const btnSpinner = document.getElementById("btnSpinnerCadastro");

        form.addEventListener("submit", async (e) => {
            e.preventDefault();

            // Inicia loading
            btn.disabled = true;
            btnText.classList.add("d-none");
            btnSpinner.classList.remove("d-none");

            const nome = document.getElementById("nome").value.trim();
            const email = document.getElementById("emailCadastro").value.trim();
            const senha = document.getElementById("senhaCadastro").value;

            try {
                const response = await fetch("/Login/CriarUsuario", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify({ nome, email, senha })
                });

                const resultado = await response.json();

                if (resultado.sucesso) {
                    mostrarToast(resultado.mensagem || "Usuário criado com sucesso!", true);

                    // Redireciona após breve delay
                    setTimeout(() => {
                        window.location.href = "/Login";
                    }, 1500);
                } else {
                    mostrarToast(resultado.mensagem || "Erro ao cadastrar usuário.", false);
                }

            } catch (err) {
                mostrarToast("Erro ao comunicar com o servidor: " + err.message, false);
            } finally {
                // Finaliza loading
                btn.disabled = false;
                btnText.classList.remove("d-none");
                btnSpinner.classList.add("d-none");
            }
        });

        document.getElementById("toggleSenhaCadastro").addEventListener("click", () => {
            const input = document.getElementById("senhaCadastro");
            const icone = document.getElementById("iconeOlhoCadastro");
            const mostrar = input.type === "password";
            input.type = mostrar ? "text" : "password";
            icone.classList.toggle("fa-eye", !mostrar);
            icone.classList.toggle("fa-eye-slash", mostrar);
        });
    }

    // ======= ESQUECI SENHA PAGE =======
    const esqueciSenhaPage = document.getElementById('esqueciSenhaPage');

    if (esqueciSenhaPage) {
        const form = document.getElementById('formEsqueciSenha');
        const emailInput = document.getElementById('emailRecuperacao');
        const btn = document.getElementById('btnEnviarEmail');
        const btnIcon = document.getElementById('btnIconEsqueci');
        const btnText = document.getElementById('btnTextEsqueci');
        const btnSpinner = document.getElementById('btnSpinnerEsqueci');

        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const email = emailInput.value.trim();
            if (!email) {
                mostrarToast("Informe seu e-mail.", "warning");
                return;
            }

            // Loading
            btn.disabled = true;
            btnIcon.classList.add("d-none");
            btnText.textContent = "Enviando...";
            btnSpinner.classList.remove("d-none");

            try {
                const response = await fetch('/Login/RedefinirSenha', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(email)
                });

                const result = await response.json();
                mostrarToast(result.mensagem, result.sucesso ? "success" : "danger");

                if (result.sucesso) {
                    setTimeout(() => window.location.href = "/Login?sucesso=senha", 2500);
                }
            } catch (err) {
                mostrarToast("Erro ao enviar instruções. Tente novamente.", "danger");
            } finally {
                btn.disabled = false;
                btnIcon.classList.remove("d-none");
                btnText.textContent = "Enviar instruções";
                btnSpinner.classList.add("d-none");
            }
        });
    }

    // ======= NOVA SENHA PAGE =======
    const novaSenhaPage = document.getElementById('novaSenhaPage');

    if (novaSenhaPage) {
        const form = document.getElementById('formNovaSenha');
        const senhaInput = document.getElementById('novaSenha');
        const toggleBtn = document.getElementById('toggleNovaSenha');
        const iconeOlho = document.getElementById('iconeOlhoNovaSenha');
        const token = document.getElementById('token').value;

        toggleBtn.addEventListener('click', () => {
            const mostrar = senhaInput.type === 'password';
            senhaInput.type = mostrar ? 'text' : 'password';
            iconeOlho.classList.toggle('fa-eye', !mostrar);
            iconeOlho.classList.toggle('fa-eye-slash', mostrar);
        });

        form.addEventListener('submit', async (e) => {
            e.preventDefault();

            const senha = senhaInput.value.trim();
            if (!senha || senha.length < 6) {
                mostrarToast("A senha deve ter no mínimo 6 caracteres.", "warning");
                return;
            }

            const btn = document.getElementById('btnNovaSenha');
            const btnIcon = document.getElementById('btnIconNovaSenha');
            const btnText = document.getElementById('btnTextNovaSenha');
            const btnSpinner = document.getElementById('btnSpinnerNovaSenha');

            // loading state
            btn.disabled = true;
            btnIcon.classList.add('d-none');
            btnText.textContent = 'Aguarde...';
            btnSpinner.classList.remove('d-none');

            try {
                const response = await fetch('/Login/SalvarNovaSenha', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ token, novaSenha: senha })
                });

                const result = await response.json();

                if (result.sucesso) {
                    mostrarToast(result.mensagem, 'success');
                    setTimeout(() => window.location.href = "/Login?sucesso=senha", 1500);
                } else {
                    mostrarToast(result.mensagem, 'danger');
                }
            } catch (err) {
                mostrarToast("Erro ao redefinir senha.", "danger");
            } finally {
                btn.disabled = false;
                btnIcon.classList.remove('d-none');
                btnText.textContent = 'Redefinir Senha';
                btnSpinner.classList.add('d-none');
            }
        });
    }

    // ======= LOGIN PAGE – Mensagem de sucesso após redefinir senha =======
    function getUrlParameter(name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        const regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        const results = regex.exec(location.search);
        return results === null ? null : decodeURIComponent(results[1].replace(/\+/g, ' '));
    }

    var meusDadosPage = document.getElementById('meusDadosPage');
    if (meusDadosPage) {

        const btnSalvar = document.querySelector('#formSalvarTudo button');
        const icon = btnSalvar?.querySelector('i');
        const text = btnSalvar?.querySelector('span');

        document.getElementById('formSalvarTudo').addEventListener('submit', async function (e) {
            e.preventDefault();

            // Animação: loading
            btnSalvar.disabled = true;
            if (icon && text) {
                icon.classList.remove('fa-save');
                icon.classList.add('spinner-border', 'spinner-border-sm');
                text.textContent = "Salvando...";
            }

            const nome = document.getElementById('nome').value;
            const email = document.getElementById('email').value;
            const senhaAtual = document.getElementById('senhaAtual').value;
            const novaSenha = document.getElementById('novaSenha').value;

            try {
                const res = await fetch('/Usuario/SalvarAlteracoes', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ nome, email, senhaAtual, novaSenha })
                });

                const resultados = await res.json();

                // Garante array de mensagens
                const mensagens = Array.isArray(resultados) ? resultados : [resultados];

                mensagens.forEach(m => mostrarToast(m.mensagem, m.sucesso));

                const sucessoSenha = mensagens.find(m => m.sucesso && m.mensagem.toLowerCase().includes('senha'));
                if (sucessoSenha) {
                    document.getElementById('senhaAtual').value = '';
                    document.getElementById('novaSenha').value = '';
                }

            } catch {
                mostrarToast("Erro ao salvar alterações. Tente novamente.", false);
            }

            // Reset animação do botão
            btnSalvar.disabled = false;
            if (icon && text) {
                icon.classList.remove('spinner-border', 'spinner-border-sm');
                icon.classList.add('fa-save');
                text.textContent = "Salvar Alterações";
            }
        });

        // Toggle de visibilidade de senha
        document.querySelectorAll('.toggle-visibility').forEach(button => {
            button.addEventListener('click', function () {
                const inputId = this.getAttribute('data-target');
                const input = document.getElementById(inputId);
                const icon = this.querySelector('i');

                if (input?.type === 'password') {
                    input.type = 'text';
                    icon?.classList.replace('fa-eye', 'fa-eye-slash');
                } else {
                    input.type = 'password';
                    icon?.classList.replace('fa-eye-slash', 'fa-eye');
                }
            });
        });

    }

    var confrontoPage = document.getElementById('confrontoPage');
    if (confrontoPage) {
        const sorteioId = document.querySelector("#confrontoPage").getAttribute("data-sorteioid");
        const times = JSON.parse(document.getElementById("timesJson")?.textContent || "[]");

        const timesOrdenados = [...times].sort((a, b) => {
            const numA = parseInt(a.Nome.match(/\d+/)?.[0] ?? 0);
            const numB = parseInt(b.Nome.match(/\d+/)?.[0] ?? 0);
            return numA - numB;
        });

        window.abrirFormularioConfronto = function () {
            document.getElementById('formularioCriarConfronto').classList.remove('d-none');
            preencherSelects();
        };

        function preencherSelects(timeASelecionado = "", timeBSelecionado = "") {
            const selectA = document.getElementById('selectTimeA');
            const selectB = document.getElementById('selectTimeB');

            selectA.innerHTML = '<option value="">Selecione um time</option>';
            selectB.innerHTML = '<option value="">Selecione um time</option>';

            timesOrdenados.forEach(t => {
                if (t.Id !== timeBSelecionado) {
                    selectA.innerHTML += `<option value="${t.Id}" ${t.Id === timeASelecionado ? 'selected' : ''}>${t.Nome}</option>`;
                }
                if (t.Id !== timeASelecionado) {
                    selectB.innerHTML += `<option value="${t.Id}" ${t.Id === timeBSelecionado ? 'selected' : ''}>${t.Nome}</option>`;
                }
            });
        }

        window.mostrarJogadores = function (letra) {
            const select = document.getElementById(`selectTime${letra}`);
            const list = document.getElementById(`jogadoresTime${letra}`);
            const outroTime = letra === "A" ? document.getElementById("selectTimeB").value : document.getElementById("selectTimeA").value;

            preencherSelects(
                letra === "A" ? select.value : outroTime,
                letra === "B" ? select.value : outroTime
            );

            const time = times.find(t => t.Id === select.value);
            list.innerHTML = '';
            if (time) {
                time.Jogadores.forEach(j => {
                    list.innerHTML += `<li class="list-group-item">${j.Nome}</li>`;
                });
            }
        };

        window.salvarConfronto = async function () {
            const timeAId = document.getElementById("selectTimeA").value;
            const timeBId = document.getElementById("selectTimeB").value;

            if (!timeAId || !timeBId || timeAId === timeBId) {
                mostrarToast("Selecione dois times diferentes.", false);
                return;
            }

            const res = await fetch(`/Confronto/Criar?sorteioId=${sorteioId}&timeAId=${timeAId}&timeBId=${timeBId}`, { method: 'POST' });
            const data = await res.json();
            mostrarToast(data.mensagem);
            await carregarConfrontos(sorteioId);
        };

        async function carregarConfrontos(sorteioId) {
            const res = await fetch(`/Confronto/ListaPorSorteio?sorteioId=${sorteioId}`);
            const confrontos = await res.json();

            const container = document.getElementById('confrontosContainer');
            container.innerHTML = '';

            confrontos.forEach(c => {
                container.innerHTML += `
<div class="col-md-6 animate__animated animate__fadeInUp">
    <div class="card shadow-sm">
        <div class="card-header fw-bold bg-dark text-white">
            ${c.timeA} 
            <span class="badge bg-success placar" data-value="${c.golsA}">0</span>
            x 
            <span class="badge bg-primary placar" data-value="${c.golsB}">0</span> 
            ${c.timeB}
        </div>
        <div class="card-body">
            <h6>${c.timeA}</h6>
            <ul class="list-group mb-2">
                ${c.jogadoresTimeA.map(j => `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <span>
                            ${c.jogadorDestaqueId === j.id ? '<i class="fas fa-star text-warning me-1"></i>' : ''}
                            ${j.nome}
                            ${j.gols > 0 ? `<span class="badge bg-success ms-1">G(${j.gols})</span>` : ''}
                            ${j.assistencias > 0 ? `<span class="badge bg-info text-dark ms-1">ASS(${j.assistencias})</span>` : ''}
                        </span>
                        <div>
                            <button class="btn btn-sm btn-success me-1" onclick="registrarGol('${c.id}', '${j.id}')">Gol</button>
                            <button class="btn btn-sm btn-info" onclick="registrarAssistencia('${c.id}', '${j.id}')">Assistência</button>
                        </div>
                    </li>`).join('')}
            </ul>
            <h6>${c.timeB}</h6>
            <ul class="list-group mb-2">
                ${c.jogadoresTimeB.map(j => `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <span>
                            ${c.jogadorDestaqueId === j.id ? '<i class="fas fa-star text-warning me-1"></i>' : ''}
                            ${j.nome}
                            ${j.gols > 0 ? `<span class="badge bg-success ms-1">G(${j.gols})</span>` : ''}
                            ${j.assistencias > 0 ? `<span class="badge bg-info text-dark ms-1">ASS(${j.assistencias})</span>` : ''}
                        </span>
                        <div>
                            <button class="btn btn-sm btn-success me-1" onclick="registrarGol('${c.id}', '${j.id}')">Gol</button>
                            <button class="btn btn-sm btn-info" onclick="registrarAssistencia('${c.id}', '${j.id}')">Assistência</button>
                        </div>
                    </li>`).join('')}
            </ul>
            <div class="text-center">
                <button class="btn btn-warning btn-sm" onclick="atualizarPlacarPrompt('${c.id}')">Atualizar Placar</button>
            </div>
        </div>
    </div>
</div>`;
            });

            document.querySelectorAll(".placar").forEach(el => {
                const final = parseInt(el.dataset.value);
                let current = 0;
                el.classList.add("animate__animated", "animate__tada");
                const interval = setInterval(() => {
                    if (current >= final) {
                        el.innerText = final;
                        clearInterval(interval);
                    } else {
                        current++;
                        el.innerText = current;
                    }
                }, 20);
            });
        }

        window.registrarGol = async function (confrontoId, jogadorId) {
            const res = await fetch(`/Confronto/AdicionarGol?confrontoId=${confrontoId}&jogadorId=${jogadorId}`, { method: "POST" });
            const data = await res.json();
            mostrarToast(data.mensagem);

            // 🎉 Confetes de 3 posições (centro e laterais inferiores)
            confetti({
                particleCount: 50,
                angle: 90,
                spread: 60,
                origin: { x: 0.5, y: 1 }
            });

            confetti({
                particleCount: 30,
                angle: 70,
                spread: 55,
                origin: { x: 0, y: 1 }
            });

            confetti({
                particleCount: 30,
                angle: 110,
                spread: 55,
                origin: { x: 1, y: 1 }
            });

            // 🔊 Som da torcida com fade-out suave
            const audio = document.getElementById("audioTorcida");
            if (audio) {
                audio.currentTime = 0;
                audio.volume = 1;
                audio.play();

                const fadeDuration = 3000;
                const fadeSteps = 30;
                const fadeInterval = fadeDuration / fadeSteps;
                let currentStep = 0;

                const fadeOut = setInterval(() => {
                    if (currentStep >= fadeSteps) {
                        clearInterval(fadeOut);
                        audio.pause();
                        audio.volume = 1;
                    } else {
                        audio.volume = 1 - currentStep / fadeSteps;
                        currentStep++;
                    }
                }, fadeInterval);
            }

            // ⚽ Bola voando (se estiver ativada)
            const bola = document.getElementById("bolaGolAnimada");
            if (bola) {
                bola.classList.remove("animar");
                void bola.offsetWidth;
                bola.classList.add("animar");
                setTimeout(() => bola.classList.remove("animar"), 1200);
            }

            // 💥 Animação no placar
            const placares = document.querySelectorAll(".placar");
            placares.forEach(p => {
                p.classList.add("animate__animated", "animate__heartBeat");
                setTimeout(() => {
                    p.classList.remove("animate__animated", "animate__heartBeat");
                }, 1000);
            });

            await carregarConfrontos(sorteioId);
        };

        window.registrarAssistencia = async function (confrontoId, jogadorId) {
            const res = await fetch(`/Confronto/AdicionarAssistencia?confrontoId=${confrontoId}&jogadorId=${jogadorId}`, { method: "POST" });
            const data = await res.json();
            mostrarToast(data.mensagem);
            await carregarConfrontos(sorteioId);
        };

        window.atualizarPlacarPrompt = async function (confrontoId) {
            const golsA = prompt("Novo placar do Time A:");
            const golsB = prompt("Novo placar do Time B:");
            if (golsA && golsB) {
                await fetch(`/Confronto/AtualizarPlacar?confrontoId=${confrontoId}&golsA=${golsA}&golsB=${golsB}`, { method: "POST" });
                mostrarToast("Placar atualizado com sucesso!");
                await carregarConfrontos(sorteioId);
            }
        };

        carregarConfrontos(sorteioId);
    }



    var criarEventoPage = document.getElementById('criarEventoPage');
    if (criarEventoPage) {
        const form = document.getElementById("formCriarEvento");
        const valorInput = document.getElementById("valorEvento");
        const botaoSalvar = form.querySelector("button[type='submit']");
        const botaoOriginal = botaoSalvar.innerHTML;

        // Máscara moeda brasileira com R$
        if (valorInput) {
            valorInput.addEventListener("input", function (e) {
                let value = e.target.value.replace(/\D/g, "");
                value = (parseInt(value || "0", 10) / 100).toFixed(2);
                value = value.replace(".", ",").replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                e.target.value = `R$ ${value}`;
            });

            valorInput.value = "R$ 0,00";
        }

        form.addEventListener("submit", async function (e) {
            e.preventDefault();

            const nome = document.getElementById("nomeEvento").value.trim();
            const tipo = parseInt(document.getElementById("tipoEvento").value);
            const data = document.getElementById("dataEvento").value;
            const valor = valorInput.value.replace("R$", "").trim().replace(/\./g, "").replace(",", ".");
            const obs = document.getElementById("obsEvento").value.trim();

            if (!nome || !data || isNaN(tipo)) {
                mostrarToast("Preencha todos os campos obrigatórios.", false);
                return;
            }

            const dto = {
                nome,
                data,
                tipo,
                valorInscricao: parseFloat(valor),
                observacoes: obs
            };

            // Ativar loading
            botaoSalvar.disabled = true;
            botaoSalvar.innerHTML = `<i class="fas fa-spinner fa-spin me-2"></i>Salvando...`;

            try {
                const res = await fetch("/Evento/Criar", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(dto)
                });

                const json = await res.json();
                if (res.ok) {
                    mostrarToast(json.mensagem, true);
                    setTimeout(() => window.location.href = "/Evento", 1500);
                } else {
                    mostrarToast(json.mensagem || "Erro ao criar evento.", false);
                    botaoSalvar.disabled = false;
                    botaoSalvar.innerHTML = botaoOriginal;
                }
            } catch (error) {
                mostrarToast("Erro ao se comunicar com o servidor.", false);
                console.error(error);
                botaoSalvar.disabled = false;
                botaoSalvar.innerHTML = botaoOriginal;
            }
        });
    }

    var detalhesEventoPage = document.getElementById('detalhesEventoPage');

    if (detalhesEventoPage) {
        // Busca especificamente o botão de "Gerar Sorteio"
        const botao = detalhesEventoPage.querySelector('a.btn-primary[href*="eventoId="]');

        if (botao) {
            const eventoId = botao.href.split("eventoId=")[1];

            botao.addEventListener('click', async function (e) {
                e.preventDefault();

                // Desativa botão e mostra loading
                botao.disabled = true;
                const originalContent = botao.innerHTML;
                botao.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>Carregando...`;

                const dataHora = new Date().toISOString();
                const local = "Local não informado";

                // 🧭 Tenta obter a localização do usuário
                if (navigator.geolocation) {
                    navigator.geolocation.getCurrentPosition(
                        async (pos) => {
                            await criarSala(pos.coords.latitude, pos.coords.longitude);
                        },
                        async (err) => {
                            console.warn("Erro ao obter localização:", err);
                            mostrarToast("Não foi possível obter sua localização. Usando coordenadas padrão.", false);
                            await criarSala(0, 0);
                        },
                        { timeout: 7000 }
                    );
                } else {
                    mostrarToast("Geolocalização não suportada neste navegador.", false);
                    await criarSala(0, 0);
                }

                async function criarSala(latitude, longitude) {
                    try {
                        const response = await fetch("/Sorteio/CriarSala", {
                            method: "POST",
                            headers: { "Content-Type": "application/x-www-form-urlencoded" },
                            body: new URLSearchParams({
                                dataHora: dataHora,
                                local: local,
                                latitude: latitude.toString(),
                                longitude: longitude.toString(),
                                eventoId: eventoId
                            })
                        });

                        const json = await response.json();

                        if (response.ok && json.redirectUrl) {
                            window.location.href = json.redirectUrl;
                        } else {
                            mostrarToast(json.mensagem || "Erro ao criar sala para o sorteio.", false);
                            botao.disabled = false;
                            botao.innerHTML = originalContent;
                        }
                    } catch (err) {
                        console.error(err);
                        mostrarToast("Erro ao se comunicar com o servidor.", false);
                        botao.disabled = false;
                        botao.innerHTML = originalContent;
                    }
                }
            });
        }
    }


});