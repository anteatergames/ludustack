var GAMEIDEAMACHINE = (function () {
    "use strict";

    var selectors = {};
    var objs = {};

    var clickSound;
    var slotsSound;

    var interval = 120;
    var stopInterval = 1200;

    var genreReady = false,
        actionReady = false,
        thingsReady = false,
        goalsReady = false,
        rulesReady = false,
        firstGenre = 0,
        firstAction = 0,
        firstThing = 0,
        firstGoal = 0,
        firstRule = 0,
        pickingGenreInterval = 0,
        stopGenreInterval = 0,
        pickingActionInterval = 0,
        stopActionInterval = 0,
        pickingThingsInterval = 0,
        stopThingsInterval = 0,
        pickingGoalsInterval = 0,
        stopGoalsInterval = 0,
        pickingRulesInterval = 0,
        stopRulesInterval = 0;

    var genre = ['beat\'em up', 'twin stick', 'management', 'sim', 'battle royale', 'dungeon crawler', 'racing', 'action', 'arcade', 'educational', 'top-down', 'adventure', 'strategy', 'rts', 'turn-based', 'role-playing', 'platformer', 'puzzle', 'visual novel', 'social media', 'mobile', 'browser', 'fighting', 'artsy', 'open world', 'tycoon', 'one touch'];

    var action = ['hide', 'evade', 'punch', 'kick', 'travel', 'carry', 'slice', 'ride', 'hunt', 'climb', 'break dance', 'skate', 'craft', 'lick', 'shoot', 'shoot at', 'play notes', 'grow', 'bounce', 'escape', 'rescue', 'go to war with', 'wage war on', 'unite', 'lead', 'build', 'destroy', 'conquer', 'invade', 'colonize', 'discover', 'explore', 'trade with', 'lead the rebels in', 'make peace with', 'investigate', 'collect gold from', 'collect crystals from', 'mine ore from', 'align', 'click on', 'match', 'throw', 'toss', 'fire pellets at', 'control', 'touch', 'stack', 'guess', 'memorize', 'rotate', 'swap', 'slide', 'avoid', 'drag and drop', 'tickle', 'race', 'challenge', 'collect', 'draw', 'unlock', 'cook', 'break', 'juggle', 'solve puzzles involving'];

    var things = ['daggers', 'boomerangs', 'monsters', 'bicycles', 'motorbikes', 'ghosts', 'rings', 'lasers', 'rusty pipes', 'life', 'dead people', 'tanks', 'punky androids', 'bikes', 'origami', 'walls', 'website', 'farm', 'ancient history', 'turtles', 'music notes', 'jigsaw pieces', 'robots', 'water', 'politicians', 'colors', 'yo momma', 'countries', 'nations', 'dragons', 'castles', 'cities', 'strongholds', 'towers', 'dungeons', 'citadels', 'kingdoms', 'bombs', 'unknown worlds', 'other worlds', 'parallel worlds', 'other dimensions', 'alien worlds', 'heaven', 'hell', 'mythological places', 'historical places', 'islands', 'sanctuaries', 'temples', 'ruins', 'factories', 'caves', 'gems', 'diamonds', 'gold nuggets', 'bricks', 'bubbles', 'squares', 'triangles', 'treasure', 'blobs', 'kitchen appliances', 'nondescript fruits', 'animals', 'birds', 'baby animals', 'farm animals', 'exotic fruits', 'sentient plants', 'your friends', 'shapes', 'jewels', 'letters', 'words', 'numbers', 'tokens', 'coins', 'eggs', 'hats', 'candy', 'chocolate', 'shoes', 'clothing items', 'princesses', 'blocks', 'cubes', 'asteroids', 'stars', 'balls', 'spheres', 'magnets', 'riddles'];

    var goals = ['to get rich', 'to fulfil a prophecy', 'to save your family', 'to impress girls', 'to get drunk', 'to gain levels', 'to build a business', 'to look cool', 'to get to the end', 'to survive', 'to win', 'for glory', 'in the name of love', 'to live forever', 'to rule the world', 'to form an empire', 'to win points', 'to reach the highscore', 'to unlock bonus items', 'to earn tokens', 'to unlock the next level', 'to become president', 'to earn discounts', 'to make your name', 'to pass exam', 'to never stop', 'to uncage a friend', 'to find the banana', 'to cook a meal'];

    var rules = ['no text', 'only 1 button', 'no UI', 'only 2 buttons', 'in 60 seconds', 'only 3 colors', 'avoid enemies', 'limited inventory', 'only one chance', 'cannot avoid', 'must fly', 'one life only', 'must not be seen', 'can\'t touch the floor', 'limited time', 'must wait', 'can\'t breath', 'object is radioactive', 'the end is near', 'no gravity', 'naked', 'limited ammo', 'must dig', 'can\'t lose', 'can\'t regenerate', 'health is depleting', 'can\'t jump', 'silently'];

    var prefixes = ['to'];

    var suffixes = ['with', 'on', 'involving', 'in', 'at', 'from'];

    function setSelectors() {
        selectors.urls = '#urls';
        selectors.gameIdeaStandalone = '#gameIdeaStandalone';
        selectors.btnGenerateGameIdea = '#btnGenerateGameIdea';
        selectors.btnSlotGenre = '.btn-slot-genre';
        selectors.btnSlotAction = '.btn-slot-action';
        selectors.btnSlotThings = '.btn-slot-things';
        selectors.btnSlotGoals = '.btn-slot-goals';
        selectors.btnSlotRules = '.btn-slot-rules';
        selectors.genre = '.game-idea-genre';
        selectors.action = '.game-idea-action';
        selectors.things = '.game-idea-things';
        selectors.goals = '.game-idea-goals';
        selectors.rules = '.game-idea-rules';
        selectors.txtCombinations = '#txtCombinations';
    }

    function cacheObjs() {
        objs.urls = $(selectors.urls);
        objs.gameIdeaStandalone = document.querySelector(selectors.gameIdeaStandalone);
        objs.btnGenerateGameIdea = document.querySelector(selectors.btnGenerateGameIdea);
        objs.btnSlotGenre = document.querySelector(selectors.btnSlotGenre);
        objs.btnSlotAction = document.querySelector(selectors.btnSlotAction);
        objs.btnSlotThings = document.querySelector(selectors.btnSlotThings);
        objs.btnSlotGoals = document.querySelector(selectors.btnSlotGoals);
        objs.btnSlotRules = document.querySelector(selectors.btnSlotRules);
        objs.genre = document.querySelector(selectors.genre);
        objs.action = document.querySelector(selectors.action);
        objs.things = document.querySelector(selectors.things);
        objs.goals = document.querySelector(selectors.goals);
        objs.rules = document.querySelector(selectors.rules);
        objs.txtCombinations = document.querySelector(selectors.txtCombinations);
    }

    function randomGenre() {
        objs.genre.classList.add("picking");
        pickingGenreInterval = setInterval(changeGenre, interval);

        stopGenreInterval = setInterval(stopPigkingGenre, stopInterval);
    }

    function changeGenre() {
        objs.genre.textContent = cleanElement(genre[firstGenre], false, false);
        firstGenre = (firstGenre + 1) % genre.length;

        setVerticalPosition(objs.genre);
    }

    function stopPigkingGenre() {
        objs.genre.classList.remove("picking");
        clearInterval(pickingGenreInterval);
        clearInterval(stopGenreInterval);

        genreReady = true;
    }

    function randomAction() {
        objs.action.classList.add("picking");
        pickingActionInterval = setInterval(changeAction, interval);

        stopActionInterval = setInterval(stopPigkingAction, stopInterval);
    }

    function changeAction() {
        objs.action.textContent = cleanElement(action[firstAction], true, false);
        firstAction = (firstAction + 1) % action.length;

        setVerticalPosition(objs.action);
    }

    function stopPigkingAction() {
        objs.action.classList.remove("picking");
        clearInterval(pickingActionInterval);
        clearInterval(stopActionInterval);

        actionReady = true;
    }

    function randomThings() {
        objs.things.classList.add("picking");
        pickingThingsInterval = setInterval(changeThings, interval);

        stopThingsInterval = setInterval(stopPigkingThings, stopInterval);
    }

    function changeThings() {
        objs.things.textContent = cleanElement(things[firstThing], false, false);
        firstThing = (firstThing + 1) % things.length;

        setVerticalPosition(objs.things);
    }

    function stopPigkingThings() {
        objs.things.classList.remove("picking");
        clearInterval(pickingThingsInterval);
        clearInterval(stopThingsInterval);

        thingsReady = true;
    }

    function randomGoals() {
        objs.goals.classList.add("picking");
        pickingGoalsInterval = setInterval(changeGoals, interval);

        stopGoalsInterval = setInterval(stopPigkingGoals, stopInterval);
    }

    function changeGoals() {
        objs.goals.textContent = cleanElement(goals[firstGoal], false, false);
        firstGoal = (firstGoal + 1) % goals.length;

        setVerticalPosition(objs.goals);
    }

    function stopPigkingGoals() {
        objs.goals.classList.remove("picking");
        clearInterval(pickingGoalsInterval);
        clearInterval(stopGoalsInterval);

        goalsReady = true;
    }

    function randomRules() {
        objs.rules.classList.add("picking");
        pickingRulesInterval = setInterval(changeRules, interval);

        stopRulesInterval = setInterval(stopPigkingRules, stopInterval);
    }

    function changeRules() {
        objs.rules.textContent = cleanElement(rules[firstRule], false, false);
        firstRule = (firstRule + 1) % rules.length;

        setVerticalPosition(objs.rules);
    }

    function stopPigkingRules() {
        objs.rules.classList.remove("picking");
        clearInterval(pickingRulesInterval);
        clearInterval(stopRulesInterval);

        rulesReady = true;
    }

    function init() {
        setSelectors();
        cacheObjs();

        bindAll();

        getElements(function () {
            if (objs.gameIdeaStandalone) {
                clickSound = new Audio("/sounds/click.mp3");
                slotsSound = new Audio("/sounds/spin.mp3");
            }

            playSlotsSound();

            pick();
        });
    }

    function bindAll() {
        bindGenerateClick();
        bindSlotGenreClick();
        bindSlotActionClick();
        bindSlotThingClick();
        bindSlotGoalClick();
        bindSlotRuleClick();
    }

    function bindGenerateClick() {
        objs.btnGenerateGameIdea.addEventListener('click', function (e) {
            e.preventDefault();

            playClickSound();

            if (!objs.gameIdeaStandalone) {
                rulesReady = true;
            }

            if (genreReady === true && actionReady === true && thingsReady === true && goalsReady === true && rulesReady === true) {
                genreReady = false;
                actionReady = false;
                thingsReady = false;
                goalsReady = false;
                rulesReady = false;

                playSlotsSound();

                pick();
            }

            return false;
        });
    }

    function bindSlotGenreClick() {
        if (objs.btnSlotGenre) {
            objs.btnSlotGenre.addEventListener('click', function (e) {
                e.preventDefault();

                playClickSound();

                if (genreReady === true) {
                    genreReady = false;

                    playSlotsSound();

                    pickGenre();
                }

                return false;
            });
        }
    }

    function bindSlotActionClick() {
        if (objs.btnSlotAction) {
            objs.btnSlotAction.addEventListener('click', function (e) {
                e.preventDefault();

                playClickSound();

                if (actionReady === true) {
                    actionReady = false;

                    playSlotsSound();

                    pickAction();
                }

                return false;
            });
        }
    }

    function bindSlotThingClick() {
        if (objs.btnSlotThings) {
            objs.btnSlotThings.addEventListener('click', function (e) {
                e.preventDefault();

                playClickSound();

                if (thingsReady === true) {
                    thingsReady = false;

                    playSlotsSound();

                    pickThing();
                }

                return false;
            });
        }
    }

    function bindSlotGoalClick() {
        if (objs.btnSlotGoals) {
            objs.btnSlotGoals.addEventListener('click', function (e) {
                e.preventDefault();

                playClickSound();

                if (goalsReady === true) {
                    goalsReady = false;

                    playSlotsSound();

                    pickGoal();
                }

                return false;
            });
        }
    }

    function bindSlotRuleClick() {
        if (objs.btnSlotRules) {
            objs.btnSlotRules.addEventListener('click', function (e) {
                e.preventDefault();

                playClickSound();

                if (rulesReady === true) {
                    rulesReady = false;

                    playSlotsSound();

                    pickRule();
                }

                return false;
            });
        }
    }

    function playSlotsSound() {
        if (objs.gameIdeaStandalone) {
            slotsSound.play();
        }
    }

    function playClickSound() {
        if (objs.gameIdeaStandalone) {
            clickSound.play();
        }
    }

    function pick() {
        genre = shuffle(genre);
        action = shuffle(action);
        things = shuffle(things);
        goals = shuffle(goals);

        if (objs.rules) {
            rules = shuffle(rules);
        }

        randomGenre();
        randomAction();
        randomThings();
        randomGoals();

        if (objs.rules) {
            randomRules();
        }
    }

    function pickGenre() {
        if (objs.genre) {
            genre = shuffle(genre);
            randomGenre();
        }
    }

    function pickAction() {
        if (objs.action) {
            action = shuffle(action);
            randomAction();
        }
    }

    function pickThing() {
        if (objs.things) {
            things = shuffle(things);
            randomThings();
        }
    }

    function pickGoal() {
        if (objs.goals) {
            goals = shuffle(goals);
            randomGoals();
        }
    }

    function pickRule() {
        if (objs.rules) {
            rules = shuffle(rules);
            randomRules();
        }
    }

    function cleanElement(str, cleanPrefix, cleanSuffix) {
        var res = str.split(" ");
        var firstWord = res[0];
        var lastWord = res[res.length - 1];

        if (res.length > 1) {
            if (objs.gameIdeaStandalone || cleanPrefix) {
                if (prefixes.indexOf(firstWord) > -1) {
                    res.shift();
                    str = res.join(" ");
                }
            }

            if (objs.gameIdeaStandalone || cleanSuffix) {
                if (suffixes.indexOf(lastWord) > -1) {
                    res.pop();
                    str = res.join(" ");
                }
            }
        }

        return str;
    }

    function shuffle(array) {
        var currentIndex = array.length, temporaryValue, randomIndex;
        while (0 !== currentIndex) {
            randomIndex = Math.floor(Math.random() * currentIndex);
            currentIndex -= 1;
            temporaryValue = array[currentIndex];
            array[currentIndex] = array[randomIndex];
            array[randomIndex] = temporaryValue;
        }

        return array;
    }

    function setVerticalPosition(el) {
        el.style.top = "34px";
        if (el.offsetHeight > 120) {
            el.style.top = "-5px";
        }
        else if (el.offsetHeight < 75) {
            el.style.top = "34px";
        }
        else {
            el.style.top = "14px";
        }
    }

    function getElements(callback) {
        var url = objs.urls.data('urlGetElements');
        if (url) {
            MAINMODULE.Ajax.Get(url, function (response) {
                if (response.success) {
                    var incomingGenres = response.value.elements.Genre;
                    if (incomingGenres.length > 0) {
                        genre = incomingGenres.map(x => x.description);
                    }

                    var incomingActions = response.value.elements.Action;
                    if (incomingActions.length > 0) {
                        action = incomingActions.map(x => x.description);
                    }

                    var incomingThings = response.value.elements.Thing;
                    if (incomingThings.length > 0) {
                        things = incomingThings.map(x => x.description);
                    }

                    var incomingGoals = response.value.elements.Goal;
                    if (incomingGoals.length > 0) {
                        goals = incomingGoals.map(x => x.description);
                    }

                    var incomingRules = response.value.elements.Rule;
                    if (incomingRules.length > 0) {
                        rules = incomingRules.map(x => x.description);
                    }

                    var incomingPrefixes = response.value.elements.Preffix;
                    if (incomingPrefixes.length > 0) {
                        prefixes = incomingPrefixes.map(x => x.description);
                    }

                    var incomingSuffixes = response.value.elements.Suffix;
                    if (incomingSuffixes.length > 0) {
                        suffixes = incomingSuffixes.map(x => x.description);
                    }

                    var combinations = genre.length * action.length * things.length * goals.length * rules.length;

                    if (objs.txtCombinations) {
                        objs.txtCombinations.textContent = new Intl.NumberFormat().format(combinations);
                    }

                    if (callback) {
                        callback();
                    }
                }
            });
        } else {
            if (callback) {
                callback();
            }
        }
    }

    return {
        Init: init
    };
}());