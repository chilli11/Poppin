import Controller from '@ember/controller';
import { action } from '@ember/object';
import { tracked } from '@glimmer/tracking';

export default class AdminLocationsLocationController extends Controller {
	queryParams = ['qr'];

	@tracked qr;

	@action
	refreshRoute(location) {
		this.transitionToRoute('locations.location', location);
	}
}
