import { module, test } from 'qunit';
import { setupTest } from 'ember-qunit';

module('Unit | Service | yelp-service', function(hooks) {
  setupTest(hooks);

  // Replace this with your real tests.
  test('it exists', function(assert) {
    let service = this.owner.lookup('service:yelp-service');
    assert.ok(service);
  });
});
