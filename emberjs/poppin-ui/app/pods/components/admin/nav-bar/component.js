import Component from '@glimmer/component'
import { inject as service } from '@ember/service';
import { action } from '@ember/object';

export default class NavBarComponent extends Component {
	@service accountService;
	@service router;

	get authorized() {
		return this.accountService.authInfo && this.accountService.authInfo.authorized;
	}

	get isAdmin() {
		const { accountInfo } = this.accountService;
		return accountInfo && accountInfo.role == 'Admin';
	}

	@action
	logout() {
		return this.accountService.logout()
			.then(this.router.transitionTo('account'));
	}
}