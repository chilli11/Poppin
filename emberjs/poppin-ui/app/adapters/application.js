import RESTAdapter from '@ember-data/adapter/rest';
import environment from 'poppin-ui/config/environment';
import { Promise } from 'rsvp';
import fetch from 'fetch';

export default class ApplicationAdapter extends RESTAdapter {
  host = environment.hostURL;
  namespace = 'api';
}
