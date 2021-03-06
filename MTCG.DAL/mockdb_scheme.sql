--
-- PostgreSQL database dump
--

-- Dumped from database version 13.4
-- Dumped by pg_dump version 13.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: uuid-ossp; Type: EXTENSION; Schema: -; Owner: -
--

CREATE EXTENSION IF NOT EXISTS "uuid-ossp" WITH SCHEMA public;


--
-- Name: EXTENSION "uuid-ossp"; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION "uuid-ossp" IS 'generate universally unique identifiers (UUIDs)';


SET default_tablespace = '';

--
-- Name: card_instances; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.card_instances (
    id uuid NOT NULL,
    card_id uuid NOT NULL
);


ALTER TABLE public.card_instances OWNER TO mocking;

--
-- Name: cards; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.cards (
    id uuid NOT NULL,
    name text NOT NULL,
    description text NOT NULL,
    type integer NOT NULL,
    damage integer NOT NULL,
    element integer NOT NULL,
    rarity integer NOT NULL,
    race integer
);


ALTER TABLE public.cards OWNER TO mocking;

--
-- Name: deck_cards; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.deck_cards (
    card_instance_id uuid,
    deck_id uuid
);


ALTER TABLE public.deck_cards OWNER TO mocking;

--
-- Name: decks; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.decks (
    id uuid NOT NULL,
    name text NOT NULL
);


ALTER TABLE public.decks OWNER TO mocking;

--
-- Name: info; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.info (
    version text NOT NULL
);


ALTER TABLE public.info OWNER TO mocking;

--
-- Name: offers; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.offers (
    id uuid NOT NULL,
    user_id uuid NOT NULL,
    offered_card_id uuid NOT NULL,
    wanted_card_id uuid NOT NULL
);


ALTER TABLE public.offers OWNER TO mocking;

--
-- Name: package_cards; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.package_cards (
    package_id uuid NOT NULL,
    card_id uuid NOT NULL
);


ALTER TABLE public.package_cards OWNER TO mocking;

--
-- Name: packages; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.packages (
    id uuid NOT NULL,
    name text NOT NULL,
    description text NOT NULL,
    cost integer NOT NULL
);


ALTER TABLE public.packages OWNER TO mocking;

--
-- Name: trades; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.trades (
    id uuid NOT NULL,
    card_one_id uuid NOT NULL,
    user_one_id uuid NOT NULL,
    user_two_id uuid NOT NULL,
    card_two_id uuid NOT NULL
);


ALTER TABLE public.trades OWNER TO mocking;

--
-- Name: user_cards; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.user_cards (
    user_id uuid NOT NULL,
    card_instance_id uuid NOT NULL
);


ALTER TABLE public.user_cards OWNER TO mocking;

--
-- Name: user_decks; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.user_decks (
    user_id uuid NOT NULL,
    deck_id uuid NOT NULL,
    main_deck boolean DEFAULT false NOT NULL
);


ALTER TABLE public.user_decks OWNER TO mocking;

--
-- Name: users; Type: TABLE; Schema: public; Owner: mocking
--

CREATE TABLE public.users (
    username text NOT NULL,
    hash text NOT NULL,
    coins integer NOT NULL,
    elo integer NOT NULL,
    admin boolean NOT NULL,
    id uuid NOT NULL,
    played_games integer DEFAULT 0 NOT NULL,
    image varchar,
    bio varchar,
    wins integer DEFAULT 0 NOT NULL
);


ALTER TABLE public.users OWNER TO mocking;

--
-- Name: card_instances card_instances_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.card_instances
    ADD CONSTRAINT card_instances_pkey PRIMARY KEY (id);


--
-- Name: card_instances card_instances_unique; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.card_instances
    ADD CONSTRAINT card_instances_unique UNIQUE (id, card_id);


--
-- Name: cards cards_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.cards
    ADD CONSTRAINT cards_pkey PRIMARY KEY (id);


--
-- Name: decks decks_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.decks
    ADD CONSTRAINT decks_pkey PRIMARY KEY (id);


--
-- Name: offers offers_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.offers
    ADD CONSTRAINT offers_pkey PRIMARY KEY (id);


--
-- Name: package_cards package_cards_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.package_cards
    ADD CONSTRAINT package_cards_pkey PRIMARY KEY (package_id, card_id);


--
-- Name: packages packages_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.packages
    ADD CONSTRAINT packages_pkey PRIMARY KEY (id);


--
-- Name: users unqiue_username; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT unqiue_username UNIQUE (username);


--
-- Name: user_cards user_cards_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.user_cards
    ADD CONSTRAINT user_cards_pkey PRIMARY KEY (user_id, card_instance_id);


--
-- Name: user_cards user_cards_unique; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.user_cards
    ADD CONSTRAINT user_cards_unique UNIQUE (user_id, card_instance_id);


--
-- Name: user_decks user_deck_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.user_decks
    ADD CONSTRAINT user_deck_pkey PRIMARY KEY (user_id, deck_id);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (id);


--
-- Name: fki_fk_deck_cards_card_instance_id; Type: INDEX; Schema: public; Owner: mocking
--

CREATE INDEX fki_fk_deck_cards_card_instance_id ON public.deck_cards USING btree (card_instance_id);


--
-- Name: fki_fk_user_deck_user_id; Type: INDEX; Schema: public; Owner: mocking
--

CREATE INDEX fki_fk_user_deck_user_id ON public.user_decks USING btree (user_id);


--
-- Name: card_instances fk_card_instances_card_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.card_instances
    ADD CONSTRAINT fk_card_instances_card_id FOREIGN KEY (card_id) REFERENCES public.cards(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: deck_cards fk_deck_cards_card_instance_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.deck_cards
    ADD CONSTRAINT fk_deck_cards_card_instance_id FOREIGN KEY (card_instance_id) REFERENCES public.card_instances(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: deck_cards fk_deck_cards_user_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.deck_cards
    ADD CONSTRAINT fk_deck_cards_user_id FOREIGN KEY (card_instance_id) REFERENCES public.card_instances(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: offers fk_offers_offered_card_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.offers
    ADD CONSTRAINT fk_offers_offered_card_id FOREIGN KEY (offered_card_id) REFERENCES public.card_instances(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: offers fk_offers_user_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.offers
    ADD CONSTRAINT fk_offers_user_id FOREIGN KEY (user_id) REFERENCES public.users(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: offers fk_offers_wanted_card_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.offers
    ADD CONSTRAINT fk_offers_wanted_card_id FOREIGN KEY (wanted_card_id) REFERENCES public.cards(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: package_cards fk_package_cards_card_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.package_cards
    ADD CONSTRAINT fk_package_cards_card_id FOREIGN KEY (card_id) REFERENCES public.cards(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: package_cards fk_package_cards_package_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.package_cards
    ADD CONSTRAINT fk_package_cards_package_id FOREIGN KEY (package_id) REFERENCES public.packages(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: trades fk_trades_card_one_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.trades
    ADD CONSTRAINT fk_trades_card_one_id FOREIGN KEY (card_one_id) REFERENCES public.card_instances(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: trades fk_trades_card_two_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.trades
    ADD CONSTRAINT fk_trades_card_two_id FOREIGN KEY (card_two_id) REFERENCES public.card_instances(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: trades fk_trades_user_one_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.trades
    ADD CONSTRAINT fk_trades_user_one_id FOREIGN KEY (user_one_id) REFERENCES public.users(id);


--
-- Name: trades fk_trades_user_two_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.trades
    ADD CONSTRAINT fk_trades_user_two_id FOREIGN KEY (user_two_id) REFERENCES public.users(id);


--
-- Name: user_cards fk_user_cards_card_instance_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.user_cards
    ADD CONSTRAINT fk_user_cards_card_instance_id FOREIGN KEY (card_instance_id) REFERENCES public.card_instances(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: user_cards fk_user_cards_user_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.user_cards
    ADD CONSTRAINT fk_user_cards_user_id FOREIGN KEY (user_id) REFERENCES public.users(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: user_decks fk_user_deck_deck_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.user_decks
    ADD CONSTRAINT fk_user_deck_deck_id FOREIGN KEY (deck_id) REFERENCES public.decks(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: user_decks fk_user_deck_user_id; Type: FK CONSTRAINT; Schema: public; Owner: mocking
--

ALTER TABLE ONLY public.user_decks
    ADD CONSTRAINT fk_user_deck_user_id FOREIGN KEY (user_id) REFERENCES public.users(id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

