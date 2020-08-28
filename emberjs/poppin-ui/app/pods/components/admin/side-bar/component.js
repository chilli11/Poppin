import Component from '@glimmer/component'
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class extends Component {
	@service accountService;
	@service router;

	get authorized() {
		return this.accountService.authInfo && this.accountService.authInfo.authorized;
	}

	@action
	logout() {
		return this.accountService.logout()
			.then(this.router.transitionTo('account.login'));
	}
}