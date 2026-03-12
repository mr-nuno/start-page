// Clock
function updateClock() {
    const now = new Date();
    const h = String(now.getHours()).padStart(2, '0');
    const m = String(now.getMinutes()).padStart(2, '0');
    const s = String(now.getSeconds()).padStart(2, '0');
    const wrap = c => c.split('').map(d => `<span class="clock-digit">${d}</span>`).join('');
    document.getElementById('clock').innerHTML = `${wrap(h)}<span class="clock-sep">:</span>${wrap(m)}<span class="clock-sep">:</span>${wrap(s)}`;

    const dateOpts = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
    document.getElementById('clock-date').textContent = now.toLocaleDateString('sv-SE', dateOpts);
}
updateClock();
setInterval(updateClock, 1000);

// Fetch helpers
async function fetchJSON(url) {
    const resp = await fetch(url);
    if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
    return resp.json();
}

function progressClass(pct) {
    if (pct < 60) return 'low';
    if (pct < 85) return 'mid';
    return 'high';
}

function dayName(dateStr) {
    const d = new Date(dateStr);
    return d.toLocaleDateString('sv-SE', { weekday: 'short' });
}

function timeAgo(dateStr) {
    if (!dateStr) return '';
    const d = new Date(dateStr);
    const now = new Date();
    const mins = Math.floor((now - d) / 60000);
    if (mins < 60) return `${mins}m ago`;
    const hrs = Math.floor(mins / 60);
    if (hrs < 24) return `${hrs}h ago`;
    return `${Math.floor(hrs / 24)}d ago`;
}

const H2 = {
    weather: '<h2><span class="icon">&#9788;</span> Luleå</h2>',
    system: '<h2><span class="icon">&#9881;</span> System</h2>',
    local: '<h2><span class="icon">&#9878;</span> Norrbotten</h2>',
    global: '<h2><span class="icon">&#127760;</span> Dagens Nyheter</h2>',
    guitar: '<h2><span class="icon">&#127928;</span> Chord of the Day</h2>',
    comic: '<h2><span class="icon">&#128214;</span> Calvin & Hobbes</h2>',
    hockey: '<h2><span class="icon">&#127954;</span> Luleå Hockey</h2>',
    song: '<h2><span class="icon">&#127925;</span> Song of the Day</h2>',
    obsidian: '<h2><span class="icon">&#128218;</span> Obsidian</h2>',
};

// Weather
async function loadWeather() {
    const card = document.getElementById('weather-card');
    try {
        const w = await fetchJSON('/api/weather');
        if (w.error) { card.innerHTML = `${H2.weather}<div class="error">${w.error}</div>`; return; }

        // Update header pill
        document.getElementById('header-temp').textContent = `${Math.round(w.temperature)}°C · ${w.description}`;

        let forecastHTML = w.forecast.map(d => `
            <div class="forecast-day">
                <div class="day-name">${dayName(d.date)}</div>
                <div class="day-icon">${d.icon}</div>
                <div class="day-temps"><span>${Math.round(d.temp_max)}°</span> / ${Math.round(d.temp_min)}°</div>
            </div>
        `).join('');

        card.innerHTML = `
            ${H2.weather}
            <div class="weather-current">
                <div class="weather-icon">${w.icon}</div>
                <div>
                    <div class="weather-temp">${Math.round(w.temperature)}°<span class="unit">C</span></div>
                    <div class="weather-desc">${w.description}</div>
                    <div class="weather-details">
                        Feels like <span>${Math.round(w.feels_like)}°</span> · Wind <span>${w.wind_speed} km/h</span> · Humidity <span>${w.humidity}%</span>
                    </div>
                </div>
            </div>
            <div class="weather-forecast">${forecastHTML}</div>
        `;
    } catch (e) {
        card.innerHTML = `${H2.weather}<div class="error">Failed to load weather</div>`;
    }
}

// System
async function loadSystem() {
    const card = document.getElementById('system-card');
    try {
        const s = await fetchJSON('/api/system');
        if (s.error) { card.innerHTML = `${H2.system}<div class="error">${s.error}</div>`; return; }

        const cpuCls = progressClass(s.cpu.percent);
        const memCls = progressClass(s.memory.percent);
        const diskCls = progressClass(s.disk.percent);

        const tempHTML = s.cpu.temp_c !== null
            ? `<div class="temp-display">${s.cpu.temp_c}°C</div>`
            : `<div class="temp-display" style="font-size:1rem;-webkit-text-fill-color:var(--text-dim)">N/A</div>`;

        card.innerHTML = `
            ${H2.system}
            <div class="sys-metric">
                <div class="sys-label"><span>CPU</span><span class="value ${cpuCls}">${s.cpu.percent}%</span></div>
                <div class="progress-bar"><div class="progress-fill ${cpuCls}" style="width:${s.cpu.percent}%"></div></div>
            </div>
            <div class="sys-metric">
                <div class="sys-label"><span>CPU Temp</span></div>
                ${tempHTML}
            </div>
            <div class="sys-metric">
                <div class="sys-label"><span>Memory</span><span class="value ${memCls}">${s.memory.used_gb} / ${s.memory.total_gb} GB · ${s.memory.percent}%</span></div>
                <div class="progress-bar"><div class="progress-fill ${memCls}" style="width:${s.memory.percent}%"></div></div>
            </div>
            <div class="sys-metric">
                <div class="sys-label"><span>Disk</span><span class="value ${diskCls}">${s.disk.used_gb} / ${s.disk.total_gb} GB · ${s.disk.percent}%</span></div>
                <div class="progress-bar"><div class="progress-fill ${diskCls}" style="width:${s.disk.percent}%"></div></div>
            </div>
        `;
    } catch (e) {
        card.innerHTML = `${H2.system}<div class="error">Failed to load system info</div>`;
    }
}

// News
async function loadNews() {
    const localCard = document.getElementById('local-news-card');
    const globalCard = document.getElementById('global-news-card');
    try {
        const n = await fetchJSON('/api/news');

        function renderNewsList(items) {
            if (!items || items.length === 0) return '<div class="error">No news available</div>';
            return `<ul class="news-list">${items.map(item => {
                const summary = item.summary ? item.summary.replace(/<[^>]*>/g, '') : '';
                const trimmed = summary.length > 150 ? summary.substring(0, 150) + '…' : summary;
                return `<li class="news-item">
                    <a href="${item.link}" target="_blank" rel="noopener">${item.title}</a>
                    ${trimmed ? `<div class="news-summary">${trimmed}</div>` : ''}
                    <div class="news-time">${timeAgo(item.published)}</div>
                </li>`;
            }).join('')}</ul>`;
        }

        localCard.innerHTML = `${H2.local}${renderNewsList(n.local)}`;
        globalCard.innerHTML = `${H2.global}${renderNewsList(n.global)}`;
    } catch (e) {
        localCard.innerHTML = `${H2.local}<div class="error">Failed to load news</div>`;
        globalCard.innerHTML = `${H2.global}<div class="error">Failed to load news</div>`;
    }
}

// Sports
async function loadSports() {
    const card = document.getElementById('sports-card');
    try {
        const s = await fetchJSON('/api/sports');
        if (s.error && !s.standings && !s.recent_games) {
            card.innerHTML = `${H2.hockey}<div class="error">Could not load hockey data</div>`;
            return;
        }

        let standingHTML = `<div class="hockey-standing">
            <div>
                <div class="team-name">${s.team}</div>
                <div class="team-record">${s.league}</div>
            </div>
        </div>`;

        if (s.standings) {
            const st = s.standings;
            standingHTML = `<div class="hockey-standing">
                <div>
                    <div class="team-name">${s.team}</div>
                    <div class="team-record">
                        ${st.rank ? `#${st.rank} in SHL` : s.league} ·
                        ${st.gp ? `${st.gp} GP` : ''}
                        ${st.w ? ` · ${st.w}W` : ''}
                        ${st.l ? ` ${st.l}L` : ''}
                        ${st.otw ? ` ${st.otw}OTW` : ''}
                        ${st.otl ? ` ${st.otl}OTL` : ''}
                    </div>
                    <div class="team-record" style="margin-top:4px">
                        ${st.gf ? `GF: ${st.gf}` : ''} ${st.ga ? `· GA: ${st.ga}` : ''}
                    </div>
                </div>
                ${st.pts ? `<div class="team-points">${st.pts}<span style="font-size:0.75rem;display:block;-webkit-text-fill-color:var(--text-dim);font-weight:700;letter-spacing:2px;margin-top:2px">PTS</span></div>` : ''}
            </div>`;
        }

        let gamesHTML = '';
        if (s.recent_games && s.recent_games.length > 0) {
            gamesHTML = `<div>
                <h3 style="font-size:0.72rem;color:var(--text-dim);margin-bottom:10px;font-weight:700;text-transform:uppercase;letter-spacing:2px">Recent Games</h3>
                <ul class="games-list">${s.recent_games.map(g => `
                    <li class="game-item">
                        <span class="game-date">${g.date}</span>
                        <span class="game-opponent">${g.opponent}</span>
                        <span class="game-score ${g.result === 'W' ? 'game-win' : 'game-loss'}">${g.score} ${g.result || ''}</span>
                    </li>
                `).join('')}</ul>
            </div>`;
        }

        let nextHTML = '';
        if (s.next_game) {
            nextHTML = `<div class="next-game">
                <div class="label">Next Game</div>
                <div class="matchup">${s.next_game.date}${s.next_game.time ? ' ' + s.next_game.time : ''} — ${s.next_game.opponent}</div>
            </div>`;
        }

        card.innerHTML = `
            ${H2.hockey}
            <div class="hockey-content">
                <div>${standingHTML}${nextHTML}</div>
                <div>${gamesHTML}</div>
            </div>
        `;
    } catch (e) {
        card.innerHTML = `${H2.hockey}<div class="error">Failed to load sports data</div>`;
    }
}

// Guitar chord
async function loadGuitar() {
    const card = document.getElementById('guitar-card');
    try {
        const g = await fetchJSON('/api/guitar');
        if (g.error) { card.innerHTML = `${H2.guitar}<div class="error">${g.error}</div>`; return; }

        card.innerHTML = `
            ${H2.guitar}
            <div class="chord-content">
                <div class="chord-diagram">${g.svg}</div>
                <div class="chord-info">
                    <div class="chord-name">${g.name}</div>
                    <div class="chord-tip">${g.tip}</div>
                </div>
            </div>
        `;
    } catch (e) {
        card.innerHTML = `${H2.guitar}<div class="error">Failed to load chord</div>`;
    }
}

// Calvin & Hobbes
async function loadComic() {
    const card = document.getElementById('comic-card');
    try {
        const c = await fetchJSON('/api/comic');

        if (c.image_url) {
            card.innerHTML = `
                ${H2.comic}
                <div class="comic-strip">
                    <img src="${c.image_url}" alt="Calvin and Hobbes - ${c.date}" loading="lazy">
                    <br><a class="comic-link" href="${c.page_url}" target="_blank" rel="noopener">View on GoComics &#8594;</a>
                </div>
            `;
        } else {
            card.innerHTML = `
                ${H2.comic}
                <div class="comic-strip">
                    <div class="error">Strip unavailable today</div>
                    <a class="comic-link" href="${c.page_url}" target="_blank" rel="noopener">View on GoComics &#8594;</a>
                </div>
            `;
        }
    } catch (e) {
        card.innerHTML = `${H2.comic}<div class="error">Failed to load comic</div>`;
    }
}

// Seinfeld quote
async function loadSeinfeld() {
    const el = document.getElementById('seinfeld-quote');
    try {
        const s = await fetchJSON('/api/seinfeld');
        el.innerHTML = `
            <div>"${s.quote}"</div>
            <div class="seinfeld-episode">— ${s.character} · ${s.episode}</div>
        `;
    } catch (e) {
        el.textContent = '';
    }
}

// Obsidian
async function loadObsidian() {
    const card = document.getElementById('obsidian-card');
    try {
        const o = await fetchJSON('/api/obsidian');

        let tasksHTML = '';
        if (o.tasks && o.tasks.length > 0) {
            tasksHTML = `<div class="obs-section">
                <div class="obs-label">Tasks</div>
                <ul class="obs-tasks">${o.tasks.map(t => `
                    <li class="obs-task" data-file="${t.file}" data-line="${t.line}">
                        <span class="obs-checkbox" onclick="toggleTask(this)">☐</span>
                        <span class="obs-task-text">${t.text}</span>
                        <span class="obs-task-file">${t.file.replace('Daily Notes/', '')}</span>
                    </li>
                `).join('')}</ul>
            </div>`;
        }

        let dailyHTML = '';
        if (o.daily_note) {
            const escaped = o.daily_note.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
            dailyHTML = `<div class="obs-section">
                <div class="obs-label">Daily Note — ${o.date}</div>
                <div class="obs-daily"><pre>${escaped}</pre></div>
            </div>`;
        }

        let recentHTML = '';
        if (o.recent_notes && o.recent_notes.length > 0) {
            recentHTML = `<div class="obs-section">
                <div class="obs-label">Recent</div>
                <div class="obs-recent">${o.recent_notes.map(n => `<span class="obs-note-tag">${n.name}</span>`).join('')}</div>
            </div>`;
        }

        card.innerHTML = `
            ${H2.obsidian}
            <div class="obs-input-row">
                <input type="text" id="obs-input" class="obs-input" placeholder="Add note or task (prefix with [ ] for task)..." />
                <button class="obs-btn" onclick="addObsidianNote()">+</button>
            </div>
            ${tasksHTML}
            ${dailyHTML}
            ${recentHTML}
        `;

        document.getElementById('obs-input').addEventListener('keydown', e => {
            if (e.key === 'Enter') addObsidianNote();
        });
    } catch (e) {
        card.innerHTML = `${H2.obsidian}<div class="error">Failed to load Obsidian</div>`;
    }
}

async function addObsidianNote() {
    const input = document.getElementById('obs-input');
    const text = input.value.trim();
    if (!text) return;

    const isTask = text.startsWith('[ ] ') || text.startsWith('[] ');
    const cleanText = isTask ? text.replace(/^\[[\s]*\]\s*/, '') : text;
    const url = isTask ? '/api/obsidian/task' : '/api/obsidian/note';

    try {
        await fetch(url, {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({text: cleanText})
        });
        input.value = '';
        loadObsidian();
    } catch (e) {
        console.error('Failed to add note:', e);
    }
}

async function toggleTask(el) {
    const li = el.closest('.obs-task');
    const file = li.dataset.file;
    const line = parseInt(li.dataset.line);
    try {
        await fetch('/api/obsidian/toggle', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({file, line})
        });
        li.classList.add('obs-task-done');
        el.textContent = '☑';
        setTimeout(loadObsidian, 500);
    } catch (e) {
        console.error('Failed to toggle task:', e);
    }
}

// Song of the day
async function loadSong() {
    const card = document.getElementById('song-card');
    try {
        const s = await fetchJSON('/api/song');
        if (s.error) { card.innerHTML = `${H2.song}<div class="error">${s.error}</div>`; return; }

        const chordsHTML = s.chords.map(c => `<span class="song-chord-tag">${c}</span>`).join('');

        let lyricsHTML = '';
        if (s.lyrics) {
            const escaped = s.lyrics.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
            lyricsHTML = `<div class="song-lyrics"><pre>${escaped}</pre></div>`;
        } else {
            lyricsHTML = `<div class="song-lyrics-unavailable">Lyrics unavailable</div>`;
        }

        card.innerHTML = `
            ${H2.song}
            <div class="song-header">
                <div class="song-title">${s.title}</div>
                <div class="song-artist">${s.artist}</div>
            </div>
            <div class="song-chords">
                <div class="song-chords-label">Chords</div>
                <div class="song-chord-tags">${chordsHTML}</div>
                <div class="song-pattern">${s.pattern}</div>
            </div>
            ${lyricsHTML}
        `;
    } catch (e) {
        card.innerHTML = `${H2.song}<div class="error">Failed to load song</div>`;
    }
}

// Quote of the day
async function loadQuote() {
    const card = document.getElementById('quote-card');
    try {
        const q = await fetchJSON('/api/quote');
        card.innerHTML = `
            <div class="daily-quote">
                <div class="daily-quote-text">"${q.quote}"</div>
                <div class="daily-quote-source">— ${q.source}</div>
            </div>
        `;
    } catch (e) {
        card.textContent = '';
    }
}

// Initial load
loadSeinfeld();
loadWeather();
loadSystem();
loadNews();
loadSports();
loadGuitar();
loadComic();
loadSong();
loadQuote();
loadObsidian();

// Refresh intervals
setInterval(loadSystem, 30000);
setInterval(loadWeather, 900000);
setInterval(loadNews, 600000);
setInterval(loadSports, 1800000);
