import RESTAdapter from '@ember-data/adapter/rest';
import environment from 'poppin-ui/config/environment';
import TokenAdapter from 'ember-simple-auth-token/mixins/token-adapter';

export default class ApplicationAdapter extends RESTAdapter.extend(TokenAdapter) {
  host = environment.hostURL;
  namespace = 'api';
}
