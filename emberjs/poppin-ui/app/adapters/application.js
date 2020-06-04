import RESTAdapter from '@ember-data/adapter/rest';
import { Promise } from 'rsvp';
import fetch from 'fetch';

export default class ApplicationAdapter extends RESTAdapter {
  host = 'https://localhost:44367';
  namespace = 'api';
}
